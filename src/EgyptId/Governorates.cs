using System.Collections.Generic;

namespace EgyptId
{
    /// <summary>
    /// Governorate (birth province) codes used in the Egyptian national ID number,
    /// positions 8-9 of the 14-digit number.
    /// </summary>
    public static class Governorates
    {
        private static readonly Dictionary<string, Governorate> _byCode = new Dictionary<string, Governorate>
        {
            { "01", new Governorate("01", "Cairo", "القاهرة") },
            { "02", new Governorate("02", "Alexandria", "الإسكندرية") },
            { "03", new Governorate("03", "Port Said", "بورسعيد") },
            { "04", new Governorate("04", "Suez", "السويس") },
            { "11", new Governorate("11", "Damietta", "دمياط") },
            { "12", new Governorate("12", "Dakahlia", "الدقهلية") },
            { "13", new Governorate("13", "Sharqia", "الشرقية") },
            { "14", new Governorate("14", "Qalyubia", "القليوبية") },
            { "15", new Governorate("15", "Kafr El Sheikh", "كفر الشيخ") },
            { "16", new Governorate("16", "Gharbia", "الغربية") },
            { "17", new Governorate("17", "Monufia", "المنوفية") },
            { "18", new Governorate("18", "Beheira", "البحيرة") },
            { "19", new Governorate("19", "Ismailia", "الإسماعيلية") },
            { "21", new Governorate("21", "Giza", "الجيزة") },
            { "22", new Governorate("22", "Beni Suef", "بني سويف") },
            { "23", new Governorate("23", "Faiyum", "الفيوم") },
            { "24", new Governorate("24", "Minya", "المنيا") },
            { "25", new Governorate("25", "Asyut", "أسيوط") },
            { "26", new Governorate("26", "Sohag", "سوهاج") },
            { "27", new Governorate("27", "Qena", "قنا") },
            { "28", new Governorate("28", "Aswan", "أسوان") },
            { "29", new Governorate("29", "Luxor", "الأقصر") },
            { "31", new Governorate("31", "Red Sea", "البحر الأحمر") },
            { "32", new Governorate("32", "New Valley", "الوادي الجديد") },
            { "33", new Governorate("33", "Matrouh", "مطروح") },
            { "34", new Governorate("34", "North Sinai", "شمال سيناء") },
            { "35", new Governorate("35", "South Sinai", "جنوب سيناء") },
            { "88", new Governorate("88", "Born abroad", "خارج الجمهورية") },
        };

        /// <summary>
        /// Looks up a governorate by its 2-digit national ID code.
        /// Returns null when the code is unknown.
        /// </summary>
        public static Governorate GetByCode(string code)
        {
            if (code == null)
                return null;

            Governorate governorate;
            return _byCode.TryGetValue(code, out governorate) ? governorate : null;
        }

        /// <summary>All known governorate codes.</summary>
        public static IEnumerable<Governorate> All
        {
            get { return _byCode.Values; }
        }
    }

    /// <summary>A governorate entry: code plus English and Arabic names.</summary>
    public class Governorate
    {
        /// <summary>2-digit code as it appears in the national ID.</summary>
        public string Code { get; }

        /// <summary>English name.</summary>
        public string NameEn { get; }

        /// <summary>Arabic name.</summary>
        public string NameAr { get; }

        public Governorate(string code, string nameEn, string nameAr)
        {
            Code = code;
            NameEn = nameEn;
            NameAr = nameAr;
        }
    }
}
