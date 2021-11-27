using MetalCore.CQS.Command;
using MetalCore.CQS.CommandQuery;
using MetalCore.CQS.Common.Results;
using MetalCore.CQS.Query;
using System.Threading;
using System.Threading.Tasks;

namespace MetalCore.CQS.Mediators
{
    /// <summary>
    /// This interface defines calling the correct command, query, or command query handler automatically.
    /// </summary>
    public interface ICqsMediator
    {
        /// <summary>
        /// Executes the command handler for the given command automatically.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <param name="token">
        /// A System.Threading.CancellationToken to observe while waiting for the task to complete.
        /// </param>
        /// <returns>An awaitable task.</returns>
        Task<IResult> ExecuteAsync(
            ICommand command,
            CancellationToken token = default);

        /// <summary>
        /// Executes the query handler for the given query automatically.
        /// </summary>
        /// <param name="query">The query to execute.</param>
        /// <param name="token">
        /// A System.Threading.CancellationToken to observe while waiting for the task to complete.
        /// </param>
        /// <returns>An awaitable task that returns the result of the query.</returns>
        Task<IResult<TResult>> ExecuteAsync<TResult>(
            ICommandQuery<TResult> query,
            CancellationToken token = default);

        /// <summary>
        /// Executes the query handler for the given query automatically.
        /// </summary>
        /// <param name="query">The query to execute.</param>
        /// <param name="token">
        /// A System.Threading.CancellationToken to observe while waiting for the task to complete.
        /// </param>
        /// <returns>An awaitable task that returns the result of the query.</returns>
        Task<IResult<TResult>> ExecuteAsync<TResult>(
            IQuery<TResult> query,
            CancellationToken token = default);
    }
}
