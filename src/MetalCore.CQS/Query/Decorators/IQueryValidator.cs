using MetalCore.CQS.Validation;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MetalCore.CQS.Query.Decorators
{
    /// <summary>
    /// This interface defines the execution of a validation check for a given query.
    /// </summary>
    /// <typeparam name="TCommandQuery">The type of query being handled.</typeparam>
    /// <typeparam name="TResult">The type of result being returned.</typeparam>
    public interface IQueryValidator<in TQuery, TResult> where TQuery : IQuery<TResult>
    {
        /// <summary>
        /// Verifies that the current query is valid.
        /// </summary>
        /// <param name="commandQuery">The query to validate.</param>
        /// <param name="token">
        /// A System.Threading.CancellationToken to observe while waiting for the task to complete.
        /// </param>
        /// <returns>An awaitable task.</returns>
        Task<IEnumerable<BrokenRule>> ValidateQueryAsync(
            TQuery commandQuery,
            CancellationToken token = default);
    }
}
