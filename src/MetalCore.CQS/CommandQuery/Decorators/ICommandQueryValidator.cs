using MetalCore.CQS.Validation;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MetalCore.CQS.CommandQuery
{
    /// <summary>
    /// This interface defines the execution of a validation check for a given command query.
    /// </summary>
    /// <typeparam name="TCommandQuery">The type of command query being handled.</typeparam>
    /// <typeparam name="TResult">The type of result being returned.</typeparam>
    public interface ICommandQueryValidator<in TCommandQuery, TResult> where TCommandQuery : ICommandQuery<TResult>
    {
        /// <summary>
        /// Verifies that the current command query is valid.
        /// </summary>
        /// <param name="commandQuery">The command query to validate.</param>
        /// <param name="token">
        /// A System.Threading.CancellationToken to observe while waiting for the task to complete.
        /// </param>
        /// <returns>An awaitable task.</returns>
        Task<IEnumerable<BrokenRule>> ValidateCommandQueryAsync(
            TCommandQuery commandQuery,
            CancellationToken token = default);
    }
}