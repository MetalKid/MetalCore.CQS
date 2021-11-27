using System;

namespace MetalCore.CQS.Query
{
    /// <summary>
    /// This interface marks a query to be cached automatically and expire on an absolute offset.
    /// </summary>
    public interface IQueryCacheableAbsoluteOffset : IQueryCacheable
    {
        /// <summary>
        /// Gets thet absolute timeout offset.
        /// </summary>
        DateTimeOffset AbsoluteTimeout { get; }
    }
}