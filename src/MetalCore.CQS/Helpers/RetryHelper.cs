using System;
using System.Linq;
using System.Reflection;

namespace MetalCore.CQS.Helpers
{
    public static class RetryHelper
    {
        /// <summary>
        /// Checks if the current exception matches any based on the parameters.
        /// </summary>
        /// <param name="checkBaseTypes">True if any of the exceptions are a base to the actual exception.</param>
        /// <param name="checkInnerExceptions">True if any of the exceptions match any inner exceptions.</param>
        /// <param name="ex">The exception that occurred while running the code.</param>
        /// <param name="onlyRetryForExceptionsOfTheseTypes">
        /// If provided, any exceptions that happen that are not in this list will 
        /// result in immediate stopage and that error will be thrown immediately.
        /// </param>
        /// <returns>True if a match is found; false otherwise.</returns>
        public static bool HasAnyExceptionMatch(bool checkBaseTypes, bool checkInnerExceptions, Exception ex,
            params Type[] onlyRetryForExceptionsOfTheseTypes)
        {
            if (ex == null)
            {
                return false;
            }

            if (!checkInnerExceptions)
            {
                return HasExceptionMatch(checkBaseTypes, ex, onlyRetryForExceptionsOfTheseTypes);
            }
            while (ex != null)
            {
                if (HasExceptionMatch(checkBaseTypes, ex, onlyRetryForExceptionsOfTheseTypes))
                {
                    return true;
                }
                ex = ex.InnerException;
            }
            return false;
        }

        /// <summary>
        /// Checks if the the current exception matches one of the given checks.
        /// </summary>
        /// <param name="checkBaseTypes">True if any of the exceptions are a base to the actual exception.</param>
        /// <param name="ex">The exception that occurred while running the code.</param>
        /// <param name="onlyRetryForExceptionsOfTheseTypes">
        /// If provided, any exceptions that happen that are not in this list will 
        /// result in immediate stopage and that error will be thrown immediately.
        /// </param>
        /// <returns>True if a match is found; false otherwise.</returns>
        private static bool HasExceptionMatch(bool checkBaseTypes, Exception ex,
            params Type[] onlyRetryForExceptionsOfTheseTypes)
        {
            Type exType = ex.GetType();
            return onlyRetryForExceptionsOfTheseTypes == null ||
                onlyRetryForExceptionsOfTheseTypes.Length == 0 ||
                onlyRetryForExceptionsOfTheseTypes.Any(
                    a =>
                        a == exType ||
                        (checkBaseTypes &&
                         (a.IsAssignableFrom(exType) || exType.GetTypeInfo().IsSubclassOf(a) ||
                          exType.IsInstanceOfType(a))));
        }
    }
}