using System.Threading;
using System.Threading.Tasks;

namespace MetalCore.CQS.Command
{
    /// <summary>
    /// This interface allows the invalidation of specific caches for a given command.
    /// </summary>
    /// <typeparam name="TCommand">The type of command being handled.</typeparam>
    public interface ICommandCacheInvalidation<in TCommand> where TCommand : ICommand
    {
        /// <summary>
        /// Invalidates any caches that this command could effect.
        /// </summary>
        /// <param name="command">The command to invalidate cache for.</param>
        /// <param name="token">
        /// A System.Threading.CancellationToken to observe while waiting for the task to complete.
        /// </param>
        /// <returns>An awaitable task with a command result.</returns>
        Task InvalidateCacheAsync(
            TCommand command,
            CancellationToken token = default);
    }
}