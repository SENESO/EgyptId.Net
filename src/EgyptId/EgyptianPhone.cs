using System.Collections.Generic;
using System.Text;

namespace EgyptId
{
    /// <summary>Mobile network operator in Egypt.</summary>
    public enum PhoneCarrier
    {
        /// <summary>Vodafone Egypt (010).</summary>
        Vodafone,

        /// <summary>Etisalat by e&amp; (011).</summary>
        Etisalat,

        /// <summary>Orange Egypt (012).</summary>
        Orange,

        /// <summary>WE by Telecom Egypt (015).</summary>
        WE
    }

    /// <summary>Kind of an Egyptian phone number.</summary>
    public enum PhoneType
    {
        /// <summary>Not a recognizable Egyptian number.</summary>
        Unknown,

        /// <summary>Mobile: 11 digits, 01x.</summary>
        Mobile,

        /// <summary>Landline: area code + subscriber.</summary>
        Landline
    }

    /// <summary>
    /// Normalization and validation of Egyptian phone numbers.
    /// Accepts local (010...), trunk (002010...), national (2010...),
    /// and international (+2010...) forms, with spaces, dashes, or
    /// Arabic-Indic digits — and normalizes everything to +20....
    /// </summary>
    public static class EgyptianPhone
    {
        /// <summary>Egypt's country code digits.</summary>
        public const string CountryCode = "20";

        private static readonly Dictionary<string, PhoneCarrier> _carriers = new Dictionary<string, PhoneCarrier>
        {
            { "10", PhoneCarrier.Vodafone },
            { "11", PhoneCarrier.Etisalat },
            { "12", PhoneCarrier.Orange },
            { "15", PhoneCarrier.WE },
        };

        // Area code (without trunk 0) -> English name.
        private static readonly Dictionary<string, string> _areaNamesEn = new Dictionary<string, string>
        {
            { "2", "Cairo / Giza" },
            { "3", "Alexandria" },
            { "13", "Benha" },
            { "40", "Tanta" },
            { "45", "Damanhur" },
            { "46", "Marsa Matruh" },
            { "47", "Kafr El Sheikh" },
            { "48", "Shibin El Kom" },
            { "50", "Mansoura" },
            { "55", "Zagazig" },
            { "57", "Damietta" },
            { "62", "Suez" },
            { "64", "Ismailia" },
            { "65", "Hurghada" },
            { "66", "Port Said" },
            { "68", "Arish" },
            { "69", "El Tor" },
            { "82", "Beni Suef" },
            { "84", "Faiyum" },
            { "86", "Minya" },
            { "88", "Asyut" },
            { "92", "New Valley" },
            { "93", "Sohag" },
            { "95", "Luxor" },
            { "96", "Qena" },
            { "97", "Aswan" },
        };

        private static readonly Dictionary<string, string> _areaNamesAr = new Dictionary<string, string>
        {
            { "2", "القاهرة / الجيزة" },
            { "3", "الإسكندرية" },
            { "13", "بنها" },
            { "40", "طنطا" },
            { "45", "دمنهور" },
            { "46", "مرسى مطروح" },
            { "47", "كفر الشيخ" },
            { "48", "شبين الكوم" },
            { "50", "المنصورة" },
            { "55", "الزقازيق" },
            { "57", "دمياط" },
            { "62", "السويس" },
            { "64", "الإسماعيلية" },
            { "65", "الغردقة" },
            { "66", "بورسعيد" },
            { "68", "العريش" },
            { "69", "الطور" },
            { "82", "بني سويف" },
            { "84", "الفيوم" },
            { "86", "المنيا" },
            { "88", "أسيوط" },
            { "92", "الوادي الجديد" },
            { "93", "سوهاج" },
            { "95", "الأقصر" },
            { "96", "قنا" },
            { "97", "أسوان" },
        };

        /// <summary>
        /// Normalizes an Egyptian number to international form (+20...).
        /// Returns null when the input is not a recognizable Egyptian number.
        /// </summary>
        /// <example>
        /// Normalize("01012345678")    // "+201012345678"
        /// Normalize("00201012345678") // "+201012345678"
        /// Normalize("+201012345678")  // "+201012345678"
        /// Normalize("0227950000")     // "+20227950000"
        /// </example>
        public static string Normalize(string phone)
        {
            if (GetPhoneType(phone) == PhoneType.Unknown)
                return null;
            var national = ToNationalNumber(phone);
            return national == null ? null : "+" + CountryCode + national;
        }

        /// <summary>
        /// True for an 11-digit Egyptian mobile number (010/011/012/015),
        /// in any accepted input form.
        /// </summary>
        public static bool IsValidMobile(string phone)
        {
            var national = ToNationalNumber(phone);
            return national != null
                && national.Length == 10
                && national[0] == '1'
                && _carriers.ContainsKey(national.Substring(0, 2));
        }

        /// <summary>
        /// The mobile carrier, or null when the number is not a valid
        /// Egyptian mobile number.
        /// </summary>
        public static PhoneCarrier? GetCarrier(string phone)
        {
            var national = ToNationalNumber(phone);
            if (national == null || national.Length != 10 || national[0] != '1')
                return null;

            PhoneCarrier carrier;
            return _carriers.TryGetValue(national.Substring(0, 2), out carrier)
                ? (PhoneCarrier?)carrier
                : null;
        }

        /// <summary>Display name of a carrier: Vodafone, Etisalat e&amp;, Orange, WE.</summary>
        public static string GetCarrierName(PhoneCarrier carrier)
        {
            switch (carrier)
            {
                case PhoneCarrier.Vodafone: return "Vodafone";
                case PhoneCarrier.Etisalat: return "Etisalat e&";
                case PhoneCarrier.Orange: return "Orange";
                case PhoneCarrier.WE: return "WE";
                default: return carrier.ToString();
            }
        }

        /// <summary>
        /// True for an Egyptian landline (known area code, 8-10 national digits),
        /// in any accepted input form.
        /// </summary>
        public static bool IsValidLandline(string phone)
        {
            var national = ToNationalNumber(phone);
            if (national == null || national[0] == '1')
                return false;

            return GetAreaCode(national) != null
                && national.Length >= 8
                && national.Length <= 10;
        }

        /// <summary>
        /// English area name for a landline number (e.g. "Cairo / Giza"),
        /// or null when the number is not a recognized landline.
        /// </summary>
        public static string GetAreaName(string phone)
        {
            return GetAreaName(phone, english: true);
        }

        /// <summary>
        /// Area name in English or Arabic for a landline number,
        /// or null when the number is not a recognized landline.
        /// </summary>
        public static string GetAreaName(string phone, bool english)
        {
            var national = ToNationalNumber(phone);
            if (national == null || national[0] == '1')
                return null;

            var areaCode = GetAreaCode(national);
            if (areaCode == null)
                return null;

            var map = english ? _areaNamesEn : _areaNamesAr;
            string name;
            return map.TryGetValue(areaCode, out name) ? name : null;
        }

        /// <summary>Classifies the number as mobile, landline, or unknown.</summary>
        public static PhoneType GetPhoneType(string phone)
        {
            if (IsValidMobile(phone))
                return PhoneType.Mobile;
            if (IsValidLandline(phone))
                return PhoneType.Landline;
            return PhoneType.Unknown;
        }

        // Strips formatting and converts any accepted form to the national
        // significant number (without trunk 0 or country code), or null.
        private static string ToNationalNumber(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return null;

            var sb = new StringBuilder(phone.Length);
            var sawPlus = false;
            foreach (var ch in phone.Trim())
            {
                if (ch == '+' && sb.Length == 0 && !sawPlus)
                {
                    sawPlus = true;
                    continue;
                }
                if (ch >= '0' && ch <= '9')
                    sb.Append(ch);
                else if (ch >= '٠' && ch <= '٩')
                    sb.Append((char)('0' + (ch - '٠')));
                else if (ch >= '۰' && ch <= '۹')
                    sb.Append((char)('0' + (ch - '۰')));
                else if (ch == ' ' || ch == '-' || ch == '(' || ch == ')')
                    continue;
                else
                    return null; // unexpected character
            }

            var digits = sb.ToString();
            if (digits.Length == 0)
                return null;

            if (sawPlus)
            {
                // +20xxxxxxxxxx
                if (!digits.StartsWith(CountryCode))
                    return null;
                return digits.Substring(2);
            }

            if (digits.StartsWith("0020"))
                return digits.Substring(4);
            if (digits.StartsWith("20") && (digits.Length == 11 || digits.Length == 12))
                return digits.Substring(2);
            if (digits.StartsWith("0"))
                return digits.Substring(1);

            return null;
        }

        // Matches the longest known area code prefix ("2"/"3" or two digits).
        private static string GetAreaCode(string national)
        {
            if (national.Length == 0)
                return null;

            if (national[0] == '2' || national[0] == '3')
                return national.Substring(0, 1);

            if (national.Length >= 2)
            {
                var two = national.Substring(0, 2);
                if (_areaNamesEn.ContainsKey(two))
                    return two;
            }

            return null;
        }
    }
}
