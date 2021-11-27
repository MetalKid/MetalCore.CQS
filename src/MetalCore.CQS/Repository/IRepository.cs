using System.Threading;
using System.Threading.Tasks;

namespace MetalCore.CQS.Repository
{
    /// <summary>
    /// This interface defines a repository that returns a result.
    /// </summary>
    /// <typeparam name="TRequest">The type of request being made.</typeparam>
    /// <typeparam name="TResult">The type of result returned.</typeparam>
    public interface IRepository<in TRequest, TResult>
    {
        /// <summary>
        /// Executes the request and returns the result.
        /// </summary>
        /// <param name="request">The request to execute.</param>
        /// <param name="token">
        /// A System.Threading.CancellationToken to observe while waiting for the task to complete.
        /// </param>
        /// <returns>An awaitable task that returns the result of the query.</returns>
        Task<TResult> ExecuteAsync(TRequest request, CancellationToken token = default);
    }

    /// <summary>
    /// This interface defines a repository that does not return a result.
    /// </summary>
    /// <typeparam name="TRequest">The type of request being made.</typeparam>
    public interface IRepository<in TRequest>
    {
        /// <summary>
        /// Executes the request.
        /// </summary>
        /// <param name="request">The request to execute.</param>
        /// <param name="token">
        /// A System.Threading.CancellationToken to observe while waiting for the task to complete.
        /// </param>
        /// <returns>An awaitable task.</returns>
        Task ExecuteAsync(TRequest request, CancellationToken token = default);
    }
}