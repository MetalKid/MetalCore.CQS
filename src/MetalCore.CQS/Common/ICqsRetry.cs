namespace MetalCore.CQS.Common
{
    /// <summary>
    /// This interface allows a CQS request to be retried up to the given number of times.
    /// </summary>
    public interface ICqsRetry
    {
        /// <summary>
        /// The maxiumum number of times to attempt a retry.
        /// </summary>
        int MaxRetries { get; }

        /// <summary>
        /// The number of milliseconds to wait between attempts.
        /// </summary>
        int RetryDelayMilliseconds { get; }
    }
}
