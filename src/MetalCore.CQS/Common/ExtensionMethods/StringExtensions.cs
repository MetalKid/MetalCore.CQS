using System.Globalization;
using System.Linq;
using System.Text;

namespace MetalCore.CQS.ExtensionMethods
{
    /// <summary>
    /// This class holds extension methods to add to the System.String class.
    /// </summary>
    /// <remarks>NOTE: We don't normally want to add methods to String, but this seems like a good one to add.</remarks>
    public static class StringExtensions
    {
        /// <summary>
        /// Replaces diacritics/accents with the simple character representations. é => e, ç => c, etc.
        /// </summary>
        /// <param name="value">The class being extended.</param>
        /// <returns>String free of diacritics/accents</returns>
        public static string RemoveDiacritics(this string value)
        {
            if (value == null)
            {
                return null;
            }

            System.Collections.Generic.IEnumerable<char> chars =
                from c in value.Normalize(NormalizationForm.FormD).ToCharArray()
                let uc = CharUnicodeInfo.GetUnicodeCategory(c)
                where uc != UnicodeCategory.NonSpacingMark
                select c;

            string cleanValue = new string(chars.ToArray()).Normalize(NormalizationForm.FormC);

            return cleanValue;
        }
    }
}