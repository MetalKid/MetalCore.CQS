using MetalCore.CQS.Common.Results;
using System.Threading;
using System.Threading.Tasks;

namespace MetalCore.CQS.Query
{
    /// <summary>
    /// This interface defines the execution of a query handler for a given query.
    /// </summary>
    /// <typeparam name="TQuery">The type of query being handled.</typeparam>
    /// <typeparam name="TResult">The type of result being returned.</typeparam>
    public interface IQueryHandler<in TQuery, TResult> where TQuery : IQuery<TResult>
    {
        /// <summary>
        /// Executes the query handler logic for the given query.
        /// </summary>
        /// <param name="query">The state of the query.</param>
        /// <param name="token">
        /// A System.Threading.CancellationToken to observe while waiting for the task to complete.
        /// </param>
        /// <returns>An awaitable task that returns the result of the query.</returns>
        Task<IResult<TResult>> ExecuteAsync(TQuery query, CancellationToken token = default);
    }
}