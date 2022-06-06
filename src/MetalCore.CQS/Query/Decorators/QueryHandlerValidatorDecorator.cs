using MetalCore.CQS.Common.Results;
using MetalCore.CQS.Validation;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MetalCore.CQS.Query.Decorators
{
    /// <summary>
    /// This class ensures the query is in a valid state to process.
    /// </summary>
    /// <typeparam name="TQuery">The type of query being handled.</typeparam>
    /// <typeparam name="TResult">The type of result being returned.</typeparam>
    public class QueryHandlerValidatorDecorator<TQuery, TResult> : IQueryHandler<TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        private readonly IQueryHandler<TQuery, TResult> _nextQueryHandler;
        private readonly IEnumerable<IQueryValidator<TQuery, TResult>> _validators;

        /// <summary>
        /// Constructor to enable IoC decoration.
        /// </summary>
        /// <param name="queryHandler">The next command handler to call in the chain.</param>
        /// <param name="validate">The validation helper.</param>
        /// <param name="validators">The list of validation classes to execute for this type of command.</param>
        public QueryHandlerValidatorDecorator(
            IQueryHandler<TQuery, TResult> queryHandler,
            ICollection<IQueryValidator<TQuery, TResult>> validators)
        {
            Guard.IsNotNull(queryHandler, nameof(queryHandler));
            Guard.IsNotNull(validators, nameof(validators));

            _nextQueryHandler = queryHandler;
            _validators = validators;
        }

        /// <summary>
        /// Ensures the command is in a valid state and then calls the next command handler in the chain.
        /// </summary>
        /// <param name="command">The state of the command.</param>
        /// <param name="token">
        /// A System.Threading.CancellationToken to observe while waiting for the task to complete.
        /// </param>
        /// <returns>An awaitable task with a command result.</returns>
        public async Task<IResult<TResult>> ExecuteAsync(TQuery query, CancellationToken token = default)
        {
            List<BrokenRule> brokenRules = (await Task.WhenAll(_validators.AsParallel()
                    .Select(a => a.ValidateQueryAsync(query, token)))
                .ConfigureAwait(false)).SelectMany(a => a).Where(a => a != null).ToList();

            if (brokenRules.Any())
            {
                return ResultHelper.ValidationError<TResult>(brokenRules);
            }

            return await _nextQueryHandler.ExecuteAsync(query, token).ConfigureAwait(false);
        }
    }
}