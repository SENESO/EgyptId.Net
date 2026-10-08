using System.Text;

namespace EgyptId
{
    /// <summary>
    /// Arabic text normalization for search and comparison: unifies common
    /// spelling variants (أإآ → ا, ة → ه, ى → ي, ؤ → و, ئ → ي) and strips
    /// diacritics and tatweel, so "أحمد" and "احمد" compare equal.
    /// </summary>
    public static class ArabicText
    {
        /// <summary>
        /// Normalizes Arabic text for search/comparison. Returns null for null input.
        /// </summary>
        public static string Normalize(string text)
        {
            if (text == null)
                return null;

            var sb = new StringBuilder(text.Length);
            var prevWasSpace = true; // also collapses leading whitespace

            foreach (var ch in text)
            {
                char mapped;
                switch (ch)
                {
                    case 'أ':
                    case 'إ':
                    case 'آ':
                        mapped = 'ا';
                        break;
                    case 'ة':
                        mapped = 'ه';
                        break;
                    case 'ى':
                        mapped = 'ي';
                        break;
                    case 'ؤ':
                        mapped = 'و';
                        break;
                    case 'ئ':
                        mapped = 'ي';
                        break;
                    case 'ـ': // tatweel U+0640
                        continue;
                    default:
                        // Diacritics: U+064B..U+0652, superscript alef U+0670.
                        if ((ch >= 'ً' && ch <= 'ْ') || ch == 'ٰ')
                            continue;
                        mapped = ch;
                        break;
                }

                var isSpace = mapped == ' ' || mapped == '\t' || mapped == '\n' || mapped == '\r';
                if (isSpace)
                {
                    if (!prevWasSpace)
                        sb.Append(' ');
                    prevWasSpace = true;
                }
                else
                {
                    sb.Append(mapped);
                    prevWasSpace = false;
                }
            }

            // Trim trailing collapsed space.
            if (sb.Length > 0 && sb[sb.Length - 1] == ' ')
                sb.Length--;

            return sb.ToString();
        }

        /// <summary>
        /// True when both strings are equal after <see cref="Normalize"/>.
        /// Null equals null; null never equals a non-null string.
        /// </summary>
        public static bool EqualsNormalized(string a, string b)
        {
            if (a == null || b == null)
                return a == null && b == null;
            return Normalize(a) == Normalize(b);
        }

        /// <summary>
        /// True when <paramref name="term"/> appears in <paramref name="text"/>
        /// after normalizing both — handy for Arabic search boxes.
        /// </summary>
        public static bool ContainsNormalized(string text, string term)
        {
            if (text == null || term == null)
                return false;
            return Normalize(text).Contains(Normalize(term));
        }
    }
}
