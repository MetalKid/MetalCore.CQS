using MetalCore.CQS.Common;
using MetalCore.CQS.Query;
using MetalCore.CQS.Validation;
using System.Threading;
using System.Threading.Tasks;

namespace MetalCore.CQS.CommandQuery
{
    /// <summary>
    /// This class handles running all of the cache invalidators at the same time on multiple threads.
    /// </summary>
    /// <typeparam name="TCommandQuery">The type of command query to invalidate caches for.</typeparam>
    /// <typeparam name="TResult">The type of result being returned.</typeparam>
    public abstract class CommandQueryCacheInvalidationBase<TCommandQuery, TResult> : InvalidateCacheBase,
        ICommandQueryCacheInvalidation<TCommandQuery, TResult> where TCommandQuery : ICommandQuery<TResult>
    {
        /// <summary>
        /// Gets the command query invalidating caches.
        /// </summary>
        protected TCommandQuery CommandQuery { get; private set; }

        /// <summary>
        /// Constructor that requires the user context.
        /// </summary>
        /// <param name="queryCacheRegion">The region key to use when caching an object.</param>
        protected CommandQueryCacheInvalidationBase(IQueryCacheRegion queryCacheRegion) : base(queryCacheRegion) { }

        /// <summary>
        /// Invalidates any caches that this command query could effect.
        /// </summary>
        /// <param name="commandQuery">The command query to invalidate cache for.</param>
        /// <param name="token">
        /// A System.Threading.CancellationToken to observe while waiting for the task to complete.
        /// </param>
        /// <returns>An awaitable task.</returns>
        public virtual Task InvalidateCacheAsync(TCommandQuery commandQuery, CancellationToken token = default)
        {
            Guard.IsNotNull(commandQuery, nameof(commandQuery));

            CommandQuery = commandQuery;

            return InvalidateCacheAsync(token);
        }
    }
}