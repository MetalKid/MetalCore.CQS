using System;

namespace MetalCore.CQS.Common
{
    /// <summary>
    /// This interface defines specific exceptions to retry a CQS request on.
    /// </summary>
    public interface ICqsRetrySpecific : ICqsRetry
    {
        /// <summary>
        /// Gets the exception types to retry. All others will not be retried.
        /// </summary>
        Type[] OnlyRetryForExceptionsOfTheseTypes { get; }

        /// <summary>
        /// Check for the specific exception based on inheritance.
        /// </summary>
        bool RetryCheckBaseTypes { get; }

        /// <summary>
        /// Check for specific exception via inner exceptions.
        /// </summary>
        bool RetryCheckInnerExceptions { get; }
    }
}