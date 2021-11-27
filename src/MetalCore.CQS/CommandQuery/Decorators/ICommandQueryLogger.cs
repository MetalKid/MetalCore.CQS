using MetalCore.CQS.Common.Results;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MetalCore.CQS.CommandQuery
{
    /// <summary>
    /// This interface allows a specific command query to do specific, custom logging.
    /// </summary>
    /// <typeparam name="TCommandQuery">The type of command query being run.</typeparam>
    /// <typeparam name="TResult">The type of result the query returns.</typeparam>
    public interface ICommandQueryLogger<in TCommandQuery, in TResult> where TCommandQuery : ICommandQuery<TResult>
    {
        /// <summary>
        /// Logs information at the start of processing a command query.
        /// </summary>
        /// <param name="commandQuery">The command query to log information for.</param>
        /// <param name="token">
        /// A System.Threading.CancellationToken to observe while waiting for the task to complete.
        /// </param>
        /// <returns>An awaitable task that returns the result of the command query.</returns>
        Task LogStartAsync(
            TCommandQuery commandQuery,
            CancellationToken token = default);

        /// <summary>
        /// Logs information when an exception occurrs during the processing of a command query.
        /// </summary>
        /// <param name="commandQuery">The command query to log information for.</param>
        /// <param name="ex">The exception that occurred while processing the command query.</param>
        /// <param name="token">
        /// A System.Threading.CancellationToken to observe while waiting for the task to complete.
        /// </param>
        /// <returns>An awaitable task that returns the result of the command query.</returns>
        Task LogErrorAsync(
            TCommandQuery commandQuery,
            Exception ex,
            CancellationToken token = default);

        /// <summary>
        /// Logs information at the end of processing a command query.
        /// </summary>
        /// <param name="commandQuery">The command query to log information for.</param>
        /// <param name="result">The result data of the command query.</param>
        /// <param name="token">
        /// A System.Threading.CancellationToken to observe while waiting for the task to complete.
        /// </param>
        /// <returns>An awaitable task that returns the result of the command query.</returns>
        Task LogEndAsync(
            TCommandQuery commandQuery,
            IResult<TResult> result,
            CancellationToken token = default);
    }
}