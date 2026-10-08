using NUnit.Framework;

namespace EgyptId.Tests
{
    [TestFixture]
    public class ArabicTextTests
    {
        [TestCase("أحمد", "احمد")]
        [TestCase("إبراهيم", "ابراهيم")]
        [TestCase("آية", "ايه")]
        [TestCase("فاطمة", "فاطمه")]
        [TestCase("مصطفى", "مصطفي")]
        [TestCase("مؤمن", "مومن")]
        [TestCase("شائعة", "شايعه")]
        [TestCase("مُحَمَّد", "محمد")]       // diacritics stripped
        [TestCase("عـربـي", "عربي")]          // tatweel stripped
        [TestCase("  أحمد   علي  ", "احمد علي")] // whitespace collapsed
        [TestCase("", "")]
        public void Normalize_UnifiesVariants(string input, string expected)
        {
            Assert.AreEqual(expected, ArabicText.Normalize(input));
        }

        [Test]
        public void Normalize_Null_ReturnsNull()
        {
            Assert.IsNull(ArabicText.Normalize(null));
        }

        [TestCase("أحمد", "احمد", true)]
        [TestCase("أحمد", "أحمـد", true)]   // tatweel ignored
        [TestCase("فاطمة", "فاطمه", true)]
        [TestCase("أحمد", "محمد", false)]
        [TestCase("أحمد", "أحمد علي", false)]
        public void EqualsNormalized_ComparesCorrectly(string a, string b, bool expected)
        {
            Assert.AreEqual(expected, ArabicText.EqualsNormalized(a, b));
        }

        [Test]
        public void EqualsNormalized_NullHandling()
        {
            Assert.IsTrue(ArabicText.EqualsNormalized(null, null));
            Assert.IsFalse(ArabicText.EqualsNormalized(null, "أحمد"));
            Assert.IsFalse(ArabicText.EqualsNormalized("أحمد", null));
        }

        [TestCase("أهلاً وسهلاً بك", "اهلا", true)]
        [TestCase("جمهورية مصر العربية", "مصر", true)]
        [TestCase("جمهورية مصر العربية", "السعودية", false)]
        public void ContainsNormalized_SearchesCorrectly(string text, string term, bool expected)
        {
            Assert.AreEqual(expected, ArabicText.ContainsNormalized(text, term));
        }
    }
}
