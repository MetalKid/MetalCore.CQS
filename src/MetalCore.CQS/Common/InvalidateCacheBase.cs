using MetalCore.CQS.Query;
using MetalCore.CQS.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MetalCore.CQS.Common
{
    /// <summary>
    /// This class handles the generic methods to invalidate the cache.
    /// </summary>
    public abstract class InvalidateCacheBase
    {
        private readonly IQueryCacheRegion _queryCacheRegion;

        /// <summary>
        /// Constructor that requires the user context.
        /// </summary>
        /// <param name="queryCacheRegion">The context of the currently logged in user.</param>
        protected InvalidateCacheBase(IQueryCacheRegion queryCacheRegion)
        {
            Guard.IsNotNull(queryCacheRegion, nameof(queryCacheRegion));

            _queryCacheRegion = queryCacheRegion;
        }

        /// <summary>
        /// Invalidates any caches that this command query could effect.
        /// </summary>
        /// <param name="token">
        /// A System.Threading.CancellationToken to observe while waiting for the task to complete.
        /// </param>
        /// <returns>An awaitable task.</returns>
        protected async Task InvalidateCacheAsync(CancellationToken token = default)
        {
            ICollection<Type> typesOfQueries = await GetTypeOfQueriesToInvalidateAsync(token).ConfigureAwait(false);
            if (typesOfQueries == null || typesOfQueries.Count == 0)
            {
                return;
            }

            await Task.WhenAll(
                typesOfQueries
                .Select(queryType => ClearRegionCacheAsync(_queryCacheRegion.GetCacheRegion(queryType), token)))
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Returns all types of queries to invalidate the cache.
        /// </summary>
        /// <param name="token">
        /// A System.Threading.CancellationToken to observe while waiting for the task to complete.
        /// </param>
        /// <returns>The types of queries to invalidate the cache for.</returns>
        protected abstract Task<ICollection<Type>> GetTypeOfQueriesToInvalidateAsync(
            CancellationToken token = default);

        /// <summary>
        /// Clears the cache for the given region.
        /// </summary>
        /// <param name="token">
        /// A System.Threading.CancellationToken to observe while waiting for the task to complete.
        /// </param>
        /// <param name="region">The region to clear the cache for.</param>
        protected abstract Task ClearRegionCacheAsync(
            string region, CancellationToken token = default);
    }
}