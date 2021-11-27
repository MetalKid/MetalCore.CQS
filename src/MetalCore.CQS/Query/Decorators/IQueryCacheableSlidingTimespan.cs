using System;

namespace MetalCore.CQS.Query
{
    /// <summary>
    /// This interface marks a query to be cached automatically and expire on a sliding timespan.
    /// </summary>
    public interface IQueryCacheableSlidingTimespan : IQueryCacheable
    {
        /// <summary>
        /// Gets the sliding timeout timespan.
        /// </summary>
        TimeSpan SlidingTimeout { get; }
    }
}