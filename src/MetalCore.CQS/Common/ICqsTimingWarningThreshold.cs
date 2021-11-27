namespace MetalCore.CQS.Common
{
    /// <summary>
    /// This interface is used to log warnings for commands that take too long to run.
    /// </summary>
    public interface ICqsTimingWarningThreshold
    {
        /// <summary>
        /// Gets the number of milliseconds it takes to run a command that would be considered a warning.
        /// </summary>
        int WarningThresholdMilliseconds { get; }
    }
}