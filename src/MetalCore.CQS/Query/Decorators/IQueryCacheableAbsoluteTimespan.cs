using System;

namespace MetalCore.CQS.Query
{
    /// <summary>
    /// This interface marks a query to be cached automatically and expire on an absolute timespan.
    /// </summary>
    public interface IQueryCacheableAbsoluteTimespan : IQueryCacheable
    {
        /// <summary>
        /// Gets the absolute timeout timespan.
        /// </summary>
        TimeSpan ExpireTimeout { get; }
    }
}