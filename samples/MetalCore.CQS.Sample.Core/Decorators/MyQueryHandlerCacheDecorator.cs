using CacheManager.Core;
using MetalCore.CQS.Common.Results;
using MetalCore.CQS.Query;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MetalCore.CQS.Sample.Core.Cache
{
    /// <summary>
    /// This class caches any query that is marked with the appropriate interface(s).
    /// </summary>
    /// <typeparam name="TQuery">The type of query being handled.</typeparam>
    /// <typeparam name="TResult">The type of result being returned.</typeparam>
    public class MyQueryHandlerCacheDecorator<TQuery, TResult> : IQueryHandler<TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        private readonly IQueryHandler<TQuery, TResult> _queryHandler;
        private readonly IQueryCacheRegion _queryCacheRegion;
        private readonly ICacheManager<object> _cache;

        /// <summary>
        /// Constructor to enable IoC decoration.
        /// </summary>
        /// <param name="queryHandler">The next query handler to call in the chain.</param>
        /// <param name="queryCacheRegion">Information about the currently logged in user.</param>
        /// <param name="cache">The caching service to store the data.</param>
        public MyQueryHandlerCacheDecorator(
            IQueryHandler<TQuery, TResult> queryHandler,
            IQueryCacheRegion queryCacheRegion,
            ICacheManager<object> cache)
        {
            _queryHandler = queryHandler;
            _queryCacheRegion = queryCacheRegion;
            _cache = cache;
        }

        /// <summary>
        /// Caches the results if the query implements IQueryCacheable and sets expirations based on other interfaces.
        /// </summary>
        /// <param name="query">The state of the query.</param>
        /// <param name="token">
        /// A System.Threading.CancellationToken to observe while waiting for the task to complete.
        /// </param>
        /// <returns>An awaitable task.</returns>
        public async Task<IResult<TResult>> ExecuteAsync(TQuery query, CancellationToken token = default)
        {
            if (!(query is IQueryCacheable))
            {
                return await _queryHandler.ExecuteAsync(query, token).ConfigureAwait(false);
            }

            string key = GetCacheKey(query);
            string region = _queryCacheRegion.GetCacheRegion(query.GetType());

            TResult result = _cache.Get<TResult>(key, region);
            if (result != null)
            {
                Console.WriteLine($"Pulled '{typeof(TQuery).FullName}' from cache.");
                return ResultHelper.Successful(result);
            }

            IResult<TResult> queryResult = await _queryHandler.ExecuteAsync(query, token).ConfigureAwait(false);
            if (!queryResult.IsSuccessful)
            {
                return queryResult;
            }

            _cache.AddOrUpdate(key, region, queryResult.Data, prev => queryResult.Data);

            UpdateExpiration(query, key, region);

            return queryResult;
        }

        /// <summary>
        /// Returns the key for the current query.
        /// </summary>
        /// <param name="query">The state of the query.</param>
        /// <returns>The key to lookup/save the results.</returns>
        private string GetCacheKey(TQuery query)
        {
            return _queryCacheRegion.GetCacheRegion(query.GetType()) +
                JsonConvert.SerializeObject(query, Formatting.None);
        }

        /// <summary>
        /// Updates the expiration of the cache, if applicable.
        /// </summary>
        /// <param name="query">The state of the query.</param>
        /// <param name="key">The key to get the data out of the cache.</param>
        /// <param name="region">The region (grouping) to get the data out of the cache.</param>
        private void UpdateExpiration(TQuery query, string key, string region)
        {
            UpdateExpireAbsoluteTimespan(query as IQueryCacheableAbsoluteTimespan, key, region);
            UpdateExpireSlidingTimespan(query as IQueryCacheableSlidingTimespan, key, region);
            UpdateExpireAbsoluteOffset(query as IQueryCacheableAbsoluteOffset, key, region);
        }

        /// <summary>
        /// Updates the expiration to become an absolute timespan.
        /// </summary>
        /// <param name="query">The state of the query.</param>
        /// <param name="key">The key to get the data out of the cache.</param>
        /// <param name="region">The region (grouping) to get the data out of the cache.</param>
        private void UpdateExpireAbsoluteTimespan(IQueryCacheableAbsoluteTimespan query, string key, string region)
        {
            if (query == null)
            {
                return;
            }

            _cache.Expire(key, region, ExpirationMode.Absolute, query.ExpireTimeout);
        }

        /// <summary>
        /// Updates the expiration to become a sliding timespan.
        /// </summary>
        /// <param name="query">The state of the query.</param>
        /// <param name="key">The key to get the data out of the cache.</param>
        /// <param name="region">The region (grouping) to get the data out of the cache.</param>
        private void UpdateExpireSlidingTimespan(IQueryCacheableSlidingTimespan query, string key, string region)
        {
            if (query == null)
            {
                return;
            }

            _cache.Expire(key, region, ExpirationMode.Sliding, query.SlidingTimeout);
        }

        /// <summary>
        /// Update the expiration to become an absolute offset.
        /// </summary>
        /// <param name="query">The state of the query.</param>
        /// <param name="key">The key to get the data out of the cache.</param>
        /// <param name="region">The region (grouping) to get the data out of the cache.</param>
        private void UpdateExpireAbsoluteOffset(IQueryCacheableAbsoluteOffset query, string key, string region)
        {
            if (query == null)
            {
                return;
            }

            _cache.Expire(key, region, query.AbsoluteTimeout);
        }
    }
}