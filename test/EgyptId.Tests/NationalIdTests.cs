using System;
using NUnit.Framework;

namespace EgyptId.Tests
{
    [TestFixture]
    public class NationalIdTests
    {
        // Male, born 1995-08-15 in Cairo: 2|95|08|15|01|0001|0
        private const string ValidMale = "29508150100010";

        // Female, born 2004-02-29 (leap day) in Alexandria: 3|04|02|29|02|0002|4
        private const string ValidFemale = "30402290200024";

        [Test]
        public void TryParse_ValidMale_DecodesEverything()
        {
            var info = NationalId.TryParse(ValidMale);

            Assert.IsTrue(info.IsValid, info.InvalidReason);
            Assert.AreEqual(ValidMale, info.Number);
            Assert.AreEqual(new DateTime(1995, 8, 15), info.BirthDate);
            Assert.AreEqual(Gender.Male, info.Gender);
            Assert.AreEqual("01", info.GovernorateCode);
            Assert.AreEqual("Cairo", info.GovernorateNameEn);
            Assert.AreEqual("القاهرة", info.GovernorateNameAr);
            Assert.AreEqual('0', info.CheckDigit);
            Assert.IsNull(info.InvalidReason);
        }

        [Test]
        public void TryParse_ValidFemale_LeapDay_DecodesGender()
        {
            var info = NationalId.TryParse(ValidFemale);

            Assert.IsTrue(info.IsValid, info.InvalidReason);
            Assert.AreEqual(new DateTime(2004, 2, 29), info.BirthDate);
            Assert.AreEqual(Gender.Female, info.Gender);
            Assert.AreEqual("02", info.GovernorateCode);
            Assert.AreEqual("الإسكندرية", info.GovernorateNameAr);
        }

        [Test]
        public void TryParse_BornAbroad_Decodes88()
        {
            // 2|90|01|01|88|0001|5
            var info = NationalId.TryParse("29001018800015");

            Assert.IsTrue(info.IsValid, info.InvalidReason);
            Assert.AreEqual("88", info.GovernorateCode);
            Assert.AreEqual("Born abroad", info.GovernorateNameEn);
            Assert.AreEqual("خارج الجمهورية", info.GovernorateNameAr);
        }

        [Test]
        public void TryParse_AllGovernorates_AreAccepted()
        {
            foreach (var gov in Governorates.All)
            {
                // 2|85|06|10|<gov>|0001|0
                var number = "2850610" + gov.Code + "00010";
                var info = NationalId.TryParse(number);
                Assert.IsTrue(info.IsValid, "Governorate " + gov.Code + ": " + info.InvalidReason);
                Assert.AreEqual(gov.NameEn, info.GovernorateNameEn);
                Assert.AreEqual(gov.NameAr, info.GovernorateNameAr);
            }
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void TryParse_NullOrEmpty_IsInvalid(string number)
        {
            var info = NationalId.TryParse(number);
            Assert.IsFalse(info.IsValid);
            Assert.IsNotNull(info.InvalidReason);
        }

        [TestCase("2950815010001")]   // 13 digits
        [TestCase("295081501000100")] // 15 digits
        [TestCase("2950815010001A")]  // non-digit
        public void TryParse_WrongShape_IsInvalid(string number)
        {
            Assert.IsFalse(NationalId.TryParse(number).IsValid);
        }

        [Test]
        public void TryParse_BadCentury_IsInvalid()
        {
            // starts with 1
            Assert.IsFalse(NationalId.TryParse("19508150100010").IsValid);
        }

        [TestCase("29513010100010")] // month 13
        [TestCase("29508320100010")] // day 32
        [TestCase("29502300100010")] // Feb 30
        [TestCase("30302290100010")] // Feb 29 on non-leap year
        public void TryParse_ImpossibleDate_IsInvalid(string number)
        {
            var info = NationalId.TryParse(number);
            Assert.IsFalse(info.IsValid, number);
            StringAssert.Contains("date", info.InvalidReason.ToLower());
        }

        [Test]
        public void TryParse_FutureBirthDate_IsInvalid()
        {
            var future = DateTime.Today.AddDays(1);
            var century = future.Year < 2000 ? "2" : "3";
            var number = century
                + (future.Year % 100).ToString("D2")
                + future.Month.ToString("D2")
                + future.Day.ToString("D2")
                + "0100010";
            Assert.IsFalse(NationalId.TryParse(number).IsValid);
        }

        [Test]
        public void TryParse_UnknownGovernorate_IsInvalid()
        {
            var info = NationalId.TryParse("29508159900010");
            Assert.IsFalse(info.IsValid);
            StringAssert.Contains("99", info.InvalidReason);
        }

        [Test]
        public void TryParse_ArabicIndicDigits_AreAccepted()
        {
            var info = NationalId.TryParse("٢٩٥٠٨١٥٠١٠٠٠١٠");
            Assert.IsTrue(info.IsValid, info.InvalidReason);
            Assert.AreEqual(ValidMale, info.Number);
            Assert.AreEqual(Gender.Male, info.Gender);
        }

        [Test]
        public void TryParse_CheckDigit_IsExposedNotEnforced()
        {
            // Same valid number with every possible last digit stays valid.
            for (var d = '0'; d <= '9'; d++)
            {
                var number = ValidMale.Substring(0, 13) + d;
                var info = NationalId.TryParse(number);
                Assert.IsTrue(info.IsValid, "digit " + d + ": " + info.InvalidReason);
                Assert.AreEqual(d, info.CheckDigit);
            }
        }

        [Test]
        public void IsValidNationalId_MatchesTryParse()
        {
            Assert.IsTrue(NationalId.IsValidNationalId(ValidMale));
            Assert.IsTrue(NationalId.IsValidNationalId(ValidFemale));
            Assert.IsFalse(NationalId.IsValidNationalId("123"));
            Assert.IsFalse(NationalId.IsValidNationalId(null));
        }

        [Test]
        public void GetAge_ComputesFullYears()
        {
            var info = NationalId.TryParse(ValidMale); // 1995-08-15
            Assert.AreEqual(31, info.GetAge(new DateTime(2026, 8, 15)));
            Assert.AreEqual(30, info.GetAge(new DateTime(2026, 1, 1)));
            Assert.AreEqual(31, info.GetAge(new DateTime(2026, 8, 16)));
        }

        [Test]
        public void GetAge_InvalidId_Throws()
        {
            var info = NationalId.TryParse("123");
            Assert.Throws<InvalidOperationException>(() => info.GetAge());
        }
    }
}
