using System;

namespace MetalCore.CQS.DateTimes
{
    /// <summary>
    /// This is the default implementation for the current date/time.
    /// </summary>
    public class DateTimeProvider : IDateTimeProvider
    {
        /// <summary>
        /// Gets the current date/time for the current timezone.
        /// </summary>
        public DateTime Now => DateTime.Now;

        /// <summary>
        /// Gets the current date/time for UTC time (GMT 0).
        /// </summary>
        public DateTime UtcNow => DateTime.UtcNow;

        /// <summary>
        /// Gets the current date for the current timezone.
        /// </summary>
        public DateTime Today => DateTime.Today;

        /// <summary>
        /// Gets the current date for UTC time (GMT 0).
        /// </summary>
        public DateTime UtcToday => UtcNow.Date;

        /// <summary>
        /// Gets the minimum date/time possible.
        /// </summary>
        public DateTime MinValue => DateTime.MinValue;

        /// <summary>
        /// Gets the maximum date/time possible.
        /// </summary>
        public DateTime MaxValue => DateTime.MaxValue;
    }
}