namespace EgyptId
{
    /// <summary>
    /// Gender encoded in an Egyptian national ID number
    /// (odd serial digit = male, even = female).
    /// </summary>
    public enum Gender
    {
        /// <summary>Male (odd digit).</summary>
        Male,

        /// <summary>Female (even digit, including 0).</summary>
        Female
    }
}
