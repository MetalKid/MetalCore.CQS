using CacheManager.Core;
using MetalCore.CQS.CommandQuery;
using MetalCore.CQS.Query;
using System.Threading;
using System.Threading.Tasks;

namespace MetalCore.CQS.Sample.Decorators
{
    public abstract class CacheManagerCommandQueryInvalidateCache<TCommandQuery, TResult> :
          CommandQueryCacheInvalidationBase<TCommandQuery, TResult> where TCommandQuery : ICommandQuery<TResult>
    {
        private readonly ICacheManager<object> _cache;

        /// <summary>
        /// Constructor that requires the user context and cache manager.
        /// </summary>
        /// <param name="queryCacheRegion">The region key to use when caching an object.</param>
        /// <param name="cache">The cache manager to invalidate entries in.</param>
        protected CacheManagerCommandQueryInvalidateCache(
            IQueryCacheRegion queryCacheRegion,
            ICacheManager<object> cache) : base(queryCacheRegion)
        {
            _cache = cache;
        }

        /// <summary>
        /// Clears the cache for the given region.
        /// </summary>
        /// <param name="region">The region to clear the cache for.</param>
        /// <param name="token">
        /// A System.Threading.CancellationToken to observe while waiting for the task to complete.
        /// </param>
        protected override Task ClearRegionCacheAsync(
            string region, CancellationToken token = default)
        {
            _cache.ClearRegion(region);
            return Task.CompletedTask;
        }
    }
}