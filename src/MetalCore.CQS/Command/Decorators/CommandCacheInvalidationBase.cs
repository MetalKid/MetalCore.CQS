using MetalCore.CQS.Common;
using MetalCore.CQS.Query;
using MetalCore.CQS.Validation;
using System.Threading;
using System.Threading.Tasks;

namespace MetalCore.CQS.Command
{
    /// <summary>
    /// This class handles running all of the cache invalidators at the same time on multiple threads.
    /// </summary>
    /// <typeparam name="TCommand">The type of command to invalidate caches for.</typeparam>
    public abstract class CommandCacheInvalidationBase<TCommand> : InvalidateCacheBase,
        ICommandCacheInvalidation<TCommand> where TCommand : ICommand
    {
        /// <summary>
        /// Gets the command invalidating caches.
        /// </summary>
        protected TCommand Command { get; private set; }

        /// <summary>
        /// Constructor that requires the user context.
        /// </summary>
        /// <param name="queryCacheRegion">The region key to use when caching an object.</param>
        protected CommandCacheInvalidationBase(IQueryCacheRegion queryCacheRegion) : base(queryCacheRegion) { }

        /// <summary>
        /// Invalidates any caches that this command could effect.
        /// </summary>
        /// <param name="command">The command to invalidate cache for.</param>
        /// <param name="token">
        /// A System.Threading.CancellationToken to observe while waiting for the task to complete.
        /// </param>
        /// <returns>An awaitable task with a command result.</returns>
        public virtual Task InvalidateCacheAsync(TCommand command, CancellationToken token = default)
        {
            Guard.IsNotNull(command, nameof(command));

            Command = command;

            return InvalidateCacheAsync(token);
        }
    }
}