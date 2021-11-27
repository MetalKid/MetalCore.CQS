using System.Threading;
using System.Threading.Tasks;

namespace MetalCore.CQS.CommandQuery
{
    /// <summary>
    /// This interface allows the invalidation of specific caches for a given command query.
    /// </summary>
    /// <typeparam name="TCommandQuery">The type of command query being handled.</typeparam>
    /// <typeparam name="TResult">The type of data being returned.</typeparam>
    public interface ICommandQueryCacheInvalidation<in TCommandQuery, TResult> where TCommandQuery : ICommandQuery<TResult>
    {
        /// <summary>
        /// Invalidates any caches that this command query could effect.
        /// </summary>
        /// <param name="commandQuery">The command query to invalidate cache for.</param>
        /// <param name="token">
        /// A System.Threading.CancellationToken to observe while waiting for the task to complete.
        /// </param>
        /// <returns>An awaitable task.</returns>
        Task InvalidateCacheAsync(
            TCommandQuery commandQuery,
            CancellationToken token = default);
    }
}