using System;

namespace MetalCore.CQS.Query
{
    /// <summary>
    /// This interface generates the region key to use when caching an object.
    /// </summary>
    public interface IQueryCacheRegion
    {
        /// <summary>
        /// Returns the region (grouping) for the give type.
        /// </summary>
        /// <param name="queryType">The type of object to cache.</param>
        /// <returns>The region (grouping) to lookup/save the results.</returns>
        string GetCacheRegion(Type queryType);
    }
}