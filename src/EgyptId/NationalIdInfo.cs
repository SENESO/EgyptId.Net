using System;

namespace EgyptId
{
    /// <summary>
    /// The parsed result of an Egyptian national ID (الرقم القومي) number.
    /// Always returned by <see cref="NationalId.TryParse"/> — check
    /// <see cref="IsValid"/> (and <see cref="InvalidReason"/>) before using
    /// the decoded fields.
    /// </summary>
    public class NationalIdInfo
    {
        /// <summary>The 14-digit number as given (Arabic-Indic digits converted).</summary>
        public string Number { get; internal set; }

        /// <summary>Whether the number passed structural validation.</summary>
        public bool IsValid { get; internal set; }

        /// <summary>Why validation failed; null when <see cref="IsValid"/> is true.</summary>
        public string InvalidReason { get; internal set; }

        /// <summary>Date of birth decoded from the number; null when invalid.</summary>
        public DateTime? BirthDate { get; internal set; }

        /// <summary>Gender decoded from the serial digit; null when invalid.</summary>
        public Gender? Gender { get; internal set; }

        /// <summary>2-digit governorate (birth province) code; null when invalid.</summary>
        public string GovernorateCode { get; internal set; }

        /// <summary>Governorate name in English; null when invalid.</summary>
        public string GovernorateNameEn { get; internal set; }

        /// <summary>Governorate name in Arabic; null when invalid.</summary>
        public string GovernorateNameAr { get; internal set; }

        /// <summary>
        /// The 14th digit. It is exposed for completeness but is NOT enforced:
        /// the civil registry describes it as an optional digit and its
        /// calculation algorithm is not officially published.
        /// </summary>
        public char CheckDigit { get; internal set; }

        /// <summary>Age in full years on the given date (defaults to today).</summary>
        public int GetAge(DateTime? onDate = null)
        {
            if (!BirthDate.HasValue)
                throw new InvalidOperationException("Cannot compute age of an invalid national ID.");

            var on = (onDate ?? DateTime.Today).Date;
            var age = on.Year - BirthDate.Value.Year;
            if (on < BirthDate.Value.AddYears(age))
                age--;
            return age;
        }
    }
}
