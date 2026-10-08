using NUnit.Framework;

namespace EgyptId.Tests
{
    [TestFixture]
    public class EgyptianPhoneTests
    {
        [TestCase("01012345678", "+201012345678")]
        [TestCase("+201012345678", "+201012345678")]
        [TestCase("00201012345678", "+201012345678")]
        [TestCase("201012345678", "+201012345678")]
        [TestCase("010 1234 5678", "+201012345678")]
        [TestCase("010-1234-5678", "+201012345678")]
        [TestCase("(010) 1234-5678", "+201012345678")]
        [TestCase("٠١٠١٢٣٤٥٦٧٨", "+201012345678")] // Arabic-Indic digits
        [TestCase("0227950000", "+20227950000")]   // Cairo landline
        [TestCase("+20227950000", "+20227950000")]
        [TestCase("034870000", "+2034870000")]     // Alexandria landline
        public void Normalize_AcceptsAllForms(string input, string expected)
        {
            Assert.AreEqual(expected, EgyptianPhone.Normalize(input));
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        [TestCase("hello")]
        [TestCase("0101234567")]      // too short
        [TestCase("010123456789")]    // too long
        [TestCase("+442071234567")]   // not Egypt
        [TestCase("01A12345678")]     // bad character
        public void Normalize_RejectsGarbage(string input)
        {
            Assert.IsNull(EgyptianPhone.Normalize(input));
        }

        [TestCase("01012345678")]
        [TestCase("01112345678")]
        [TestCase("01212345678")]
        [TestCase("01512345678")]
        [TestCase("+201112345678")]
        [TestCase("00201212345678")]
        public void IsValidMobile_AcceptsAllCarriers(string input)
        {
            Assert.IsTrue(EgyptianPhone.IsValidMobile(input), input);
        }

        [TestCase("01912345678")] // unknown prefix
        [TestCase("0101234567")]  // 10 digits
        [TestCase("0227950000")]  // landline
        [TestCase(null)]
        public void IsValidMobile_RejectsOthers(string input)
        {
            Assert.IsFalse(EgyptianPhone.IsValidMobile(input));
        }

        [TestCase("01012345678", PhoneCarrier.Vodafone)]
        [TestCase("01112345678", PhoneCarrier.Etisalat)]
        [TestCase("01212345678", PhoneCarrier.Orange)]
        [TestCase("01512345678", PhoneCarrier.WE)]
        [TestCase("+201512345678", PhoneCarrier.WE)]
        public void GetCarrier_DetectsAll(string input, PhoneCarrier expected)
        {
            Assert.AreEqual(expected, EgyptianPhone.GetCarrier(input));
        }

        [TestCase("01912345678")]
        [TestCase("0227950000")]
        [TestCase(null)]
        public void GetCarrier_ReturnsNullForNonMobile(string input)
        {
            Assert.IsNull(EgyptianPhone.GetCarrier(input));
        }

        [Test]
        public void GetCarrierName_ReturnsDisplayNames()
        {
            Assert.AreEqual("Vodafone", EgyptianPhone.GetCarrierName(PhoneCarrier.Vodafone));
            Assert.AreEqual("Etisalat e&", EgyptianPhone.GetCarrierName(PhoneCarrier.Etisalat));
            Assert.AreEqual("Orange", EgyptianPhone.GetCarrierName(PhoneCarrier.Orange));
            Assert.AreEqual("WE", EgyptianPhone.GetCarrierName(PhoneCarrier.WE));
        }

        [TestCase("0227950000")]   // Cairo
        [TestCase("034870000")]    // Alexandria
        [TestCase("0401234567")]   // Tanta
        [TestCase("+20501234567")] // Mansoura
        public void IsValidLandline_AcceptsKnownAreas(string input)
        {
            Assert.IsTrue(EgyptianPhone.IsValidLandline(input), input);
        }

        [TestCase("01012345678")] // mobile
        [TestCase("0991234567")]  // unknown area
        [TestCase(null)]
        public void IsValidLandline_RejectsOthers(string input)
        {
            Assert.IsFalse(EgyptianPhone.IsValidLandline(input));
        }

        [TestCase("0227950000", "Cairo / Giza")]
        [TestCase("034870000", "Alexandria")]
        [TestCase("0401234567", "Tanta")]
        [TestCase("0881234567", "Asyut")]
        [TestCase("0971234567", "Aswan")]
        public void GetAreaName_ReturnsEnglishName(string input, string expected)
        {
            Assert.AreEqual(expected, EgyptianPhone.GetAreaName(input));
        }

        [Test]
        public void GetAreaName_Arabic_ReturnsArabicName()
        {
            Assert.AreEqual("القاهرة / الجيزة", EgyptianPhone.GetAreaName("0227950000", english: false));
            Assert.AreEqual("الإسكندرية", EgyptianPhone.GetAreaName("034870000", english: false));
            Assert.AreEqual("أسيوط", EgyptianPhone.GetAreaName("0881234567", english: false));
        }

        [TestCase("01012345678")] // mobile has no area
        [TestCase("0991234567")]  // unknown area
        [TestCase(null)]
        public void GetAreaName_ReturnsNullWhenUnknown(string input)
        {
            Assert.IsNull(EgyptianPhone.GetAreaName(input));
        }

        [Test]
        public void GetPhoneType_ClassifiesCorrectly()
        {
            Assert.AreEqual(PhoneType.Mobile, EgyptianPhone.GetPhoneType("01012345678"));
            Assert.AreEqual(PhoneType.Mobile, EgyptianPhone.GetPhoneType("+201512345678"));
            Assert.AreEqual(PhoneType.Landline, EgyptianPhone.GetPhoneType("0227950000"));
            Assert.AreEqual(PhoneType.Unknown, EgyptianPhone.GetPhoneType("0991234567"));
            Assert.AreEqual(PhoneType.Unknown, EgyptianPhone.GetPhoneType("hello"));
        }
    }
}
