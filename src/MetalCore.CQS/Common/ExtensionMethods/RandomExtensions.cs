using MetalCore.CQS.Validation;
using System;
using System.Text;

namespace MetalCore.CQS.ExtensionMethods
{
    /// <summary>
    /// This class holds extension methods to add to the System.Random class.
    /// </summary>
    public static class RandomExtensions
    {
        private const int InvalidIndex = -1;

        /// <summary>
        /// Returns a random string of the given length with a few options for the generation.
        /// </summary>
        /// <param name="random">The class being extended.</param>
        /// <param name="size">The length of the string.</param>
        /// <param name="requiresAtLeastOneNumber">
        /// True if at least one number is generated; false otherwise. Default is true.
        /// </param>
        /// <param name="requiresAtLeastOneUpperCaseLetter">
        /// True if at least one upper case number is required; false otherwise. Default is true.
        /// </param>
        /// <returns>Random string.</returns>
        public static string NextString(
            this Random random,
            int size,
            bool requiresAtLeastOneNumber = true,
            bool requiresAtLeastOneUpperCaseLetter = true)
        {
            Guard.IsGreaterThan(size, 0, nameof(size));
            if (requiresAtLeastOneNumber && requiresAtLeastOneUpperCaseLetter)
            {
                Guard.IsGreaterThan(size, 1, nameof(size));
            }

            byte[] data = new byte[size];
            int randomIndexNumber = !requiresAtLeastOneNumber ? InvalidIndex : random.Next(0, data.Length);
            int randomIndexUpper = !requiresAtLeastOneUpperCaseLetter ? InvalidIndex : random.Next(0, data.Length);
            while (requiresAtLeastOneNumber && randomIndexUpper == randomIndexNumber)
            {
                randomIndexUpper = random.Next(0, data.Length);
            }

            for (int i = 0; i < data.Length; i++)
            {
                byte result = 0;
                if (requiresAtLeastOneNumber && i == randomIndexNumber)
                {
                    result = (byte)random.Next(48, 58);
                }
                else if (requiresAtLeastOneUpperCaseLetter && i == randomIndexUpper)
                {
                    result = (byte)random.Next(65, 91);
                }
                else
                {
                    while (result < 48 || result > 122 || (result > 57 && result < 65) || (result > 90 && result < 97))
                    {
                        result = (byte)random.Next(48, 123);
                    }
                }
                data[i] = result;
            }
            ASCIIEncoding encoding = new ASCIIEncoding();
            string response = encoding.GetString(data);
            return response;
        }
    }
}