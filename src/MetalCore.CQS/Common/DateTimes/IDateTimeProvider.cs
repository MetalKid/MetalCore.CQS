using System;

namespace MetalCore.CQS.DateTimes
{
    /// <summary>
    /// This interface defines properties needed to get the current date/time.
    /// This allows the current date/time to be mocked for unit tests.
    /// </summary>
    public interface IDateTimeProvider
    {
        /// <summary>
        /// Gets the current date/time for the current timezone.
        /// </summary>
        DateTime Now { get; }

        /// <summary>
        /// Gets the current date/time for UTC time (GMT 0).
        /// </summary>
        DateTime UtcNow { get; }

        /// <summary>
        /// Gets the current date for the current timezone.
        /// </summary>
        DateTime Today { get; }

        /// <summary>
        /// Gets the current date for UTC time (GMT 0).
        /// </summary>
        DateTime UtcToday { get; }

        /// <summary>
        /// Gets the minimum date/time possible.
        /// </summary>
        DateTime MinValue { get; }

        /// <summary>
        /// Gets the maximum date/time possible.
        /// </summary>
        DateTime MaxValue { get; }
    }
}