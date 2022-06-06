using MetalCore.CQS.Validation;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MetalCore.CQS.Query.Decorators
{
    /// <summary>
    /// This class handles running all the rules (sync and async) at the same time on multiple threads.
    /// </summary>
    /// <typeparam name="TQuery">The type of query being validated.</typeparam>
    /// <typeparam name="TResult">The type of result being returned.</typeparam>
    public abstract class QueryValidatorBase<TQuery, TResult> : ValidatorBase<TQuery>,
        IQueryValidator<TQuery, TResult> where TQuery : IQuery<TResult>
    {
        /// <summary>
        /// Gets the command query being validated.
        /// </summary>
        protected TQuery Query { get; private set; }

        /// <summary>
        /// Ensures that the query is valid.
        /// </summary>
        /// <param name="query">The query to validate.</param>
        /// <param name="validate">The validation helper.</param>
        /// <param name="token">
        /// A System.Threading.CancellationToken to observe while waiting for the task to complete.
        /// </param>
        /// <returns>Any broken rules that were found.</returns>
        public async Task<IEnumerable<BrokenRule>> ValidateQueryAsync(
            TQuery query,
            CancellationToken token = default)
        {
            Guard.IsNotNull(query, nameof(query));

            Query = query;

            CreateRules(query, token);

            return await ValidateRulesAsync().ConfigureAwait(false);
        }
    }
}
