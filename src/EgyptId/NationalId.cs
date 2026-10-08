using System;
using System.Text;

namespace EgyptId
{
    /// <summary>
    /// Validation and parsing of the Egyptian national ID number (الرقم القومي).
    /// <para>
    /// Layout of the 14 digits: [century][YY][MM][DD][governorate 2 digits]
    /// [serial 4 digits][check digit]. Century digit 2 = 1900s, 3 = 2000s.
    /// An odd serial digit means male, an even one female.
    /// </para>
    /// <para>
    /// Note on the check digit: the civil registry describes the 14th digit as
    /// optional and its calculation algorithm is not officially published, so
    /// this library validates structure, date, and governorate — but deliberately
    /// does NOT enforce the check digit. It is exposed on
    /// <see cref="NationalIdInfo.CheckDigit"/> for completeness.
    /// </para>
    /// </summary>
    public static class NationalId
    {
        /// <summary>
        /// Validates the number and, when valid, decodes birth date, gender,
        /// and governorate. Never returns null — check
        /// <see cref="NationalIdInfo.IsValid"/>.
        /// </summary>
        public static NationalIdInfo TryParse(string number)
        {
            var info = new NationalIdInfo();

            if (string.IsNullOrWhiteSpace(number))
                return Fail(info, null, "Number is required.");

            var digits = ToAsciiDigits(number.Trim());

            if (digits.Length != 14)
                return Fail(info, digits, "National ID must be exactly 14 digits.");

            foreach (var ch in digits)
            {
                if (ch < '0' || ch > '9')
                    return Fail(info, digits, "National ID must contain digits only.");
            }

            info.Number = digits;
            info.CheckDigit = digits[13];

            // Century: 2 -> 1900s, 3 -> 2000s.
            var centuryDigit = digits[0] - '0';
            int centuryBase;
            if (centuryDigit == 2)
                centuryBase = 1900;
            else if (centuryDigit == 3)
                centuryBase = 2000;
            else
                return Fail(info, digits, "First digit must be 2 (1900s) or 3 (2000s).");

            // Date of birth: YY MM DD.
            var year = centuryBase + int.Parse(digits.Substring(1, 2));
            var month = int.Parse(digits.Substring(3, 2));
            var day = int.Parse(digits.Substring(5, 2));

            DateTime birthDate;
            try
            {
                birthDate = new DateTime(year, month, day);
            }
            catch (ArgumentOutOfRangeException)
            {
                return Fail(info, digits, "Birth date encoded in the number is not a real date.");
            }

            if (birthDate.Date > DateTime.Today)
                return Fail(info, digits, "Birth date cannot be in the future.");

            // Governorate code (positions 8-9).
            var governorateCode = digits.Substring(7, 2);
            var governorate = Governorates.GetByCode(governorateCode);
            if (governorate == null)
                return Fail(info, digits, "Unknown governorate code '" + governorateCode + "'.");

            // Gender from the last serial digit (position 13): odd = male, even = female.
            var genderDigit = digits[12] - '0';

            info.BirthDate = birthDate;
            info.Gender = genderDigit % 2 == 1 ? Gender.Male : Gender.Female;
            info.GovernorateCode = governorate.Code;
            info.GovernorateNameEn = governorate.NameEn;
            info.GovernorateNameAr = governorate.NameAr;
            info.IsValid = true;
            return info;
        }

        /// <summary>True when the number passes structural validation.</summary>
        public static bool IsValidNationalId(string number)
        {
            return TryParse(number).IsValid;
        }

        private static NationalIdInfo Fail(NationalIdInfo info, string number, string reason)
        {
            info.Number = number;
            info.IsValid = false;
            info.InvalidReason = reason;
            return info;
        }

        /// <summary>
        /// Converts Arabic-Indic digits (٠-٩) and Eastern Arabic-Indic digits
        /// (۰-۹) to ASCII digits so users can paste numbers typed in Arabic.
        /// </summary>
        internal static string ToAsciiDigits(string value)
        {
            var sb = new StringBuilder(value.Length);
            foreach (var ch in value)
            {
                if (ch >= '٠' && ch <= '٩')          // U+0660..U+0669
                    sb.Append((char)('0' + (ch - '٠')));
                else if (ch >= '۰' && ch <= '۹')     // U+06F0..U+06F9
                    sb.Append((char)('0' + (ch - '۰')));
                else
                    sb.Append(ch);
            }
            return sb.ToString();
        }
    }
}
