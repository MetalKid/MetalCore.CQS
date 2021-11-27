using MetalCore.CQS.Common.Results;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MetalCore.CQS.Query
{
    /// <summary>
    /// This interface allows a specific query to do specific, custom logging.
    /// </summary>
    /// <typeparam name="TQuery">The type of query being run.</typeparam>
    /// <typeparam name="TResult">The type of result the query returns.</typeparam>
    public interface IQueryLogger<in TQuery, in TResult> where TQuery : IQuery<TResult>
    {
        /// <summary>
        /// Logs information at the start of processing a query.
        /// </summary>
        /// <param name="query">The query to log information for.</param>
        /// <param name="token">
        /// A System.Threading.CancellationToken to observe while waiting for the task to complete.
        /// </param>
        /// <returns>An awaitable task that returns the result of the query.</returns>
        Task LogStartAsync(
            TQuery query,
            CancellationToken token = default);

        /// <summary>
        /// Logs information when an exception occurrs during the processing of a query.
        /// </summary>
        /// <param name="query">The query to log information for.</param>
        /// <param name="ex">The exception that occurred while processing the query.</param>
        /// <param name="token">
        /// A System.Threading.CancellationToken to observe while waiting for the task to complete.
        /// </param>
        /// <returns>An awaitable task that returns the result of the query.</returns>
        Task LogErrorAsync(
            TQuery query,
            Exception ex,
            CancellationToken token = default);

        /// <summary>
        /// Logs information at the end of processing a query.
        /// </summary>
        /// <param name="query">The query to log information for.</param>
        /// <param name="result">The result data of the query.</param>
        /// <param name="token">
        /// A System.Threading.CancellationToken to observe while waiting for the task to complete.
        /// </param>
        /// <returns>An awaitable task that returns the result of the query.</returns>
        Task LogEndAsync(
            TQuery query,
            IResult<TResult> result,
            CancellationToken token = default);
    }
}