using System.Threading;
using System.Threading.Tasks;

namespace MetalCore.CQS.Mediators
{
    /// <summary>
    /// This interface defines calling the correct repository automatically.
    /// </summary>
    public interface IRepositoryMediator
    {
        /// <summary>
        /// Executes the repository for the given request automatically.
        /// </summary>
        /// <typeparam name="TResult">The type of data returned.</typeparam>
        /// <param name="request">The request to execute.</param>
        /// <param name="token">
        /// A System.Threading.CancellationToken to observe while waiting for the task to complete.
        /// </param>
        /// <returns>An awaitable task that returns a result.</returns>
        Task<TResult> ExecuteAsync<TResult>(
            object request,
            CancellationToken token = default);

        /// <summary>
        /// Executes the repository for the given request automatically.
        /// </summary>
        /// <param name="request">The request to execute.</param>
        /// <param name="token">
        /// A System.Threading.CancellationToken to observe while waiting for the task to complete.
        /// </param>
        /// <returns>An awaitable task.</returns>
        Task ExecuteAsync(
            object request,
            CancellationToken token = default);
    }
}
