using MetalCore.CQS.Common.Results;
using MetalCore.CQS.Validation;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MetalCore.CQS.CommandQuery
{
    /// <summary>
    /// This class ensures the command query is in a valid state to process.
    /// </summary>
    /// <typeparam name="TCommandQuery">The type of command query being handled.</typeparam>
    /// <typeparam name="TResult">The type of result being returned.</typeparam>
    public class CommandQueryHandlerValidatorDecorator<TCommandQuery, TResult> :
        ICommandQueryHandler<TCommandQuery, TResult> where TCommandQuery : ICommandQuery<TResult>
    {
        private readonly ICommandQueryHandler<TCommandQuery, TResult> _commandQueryHandler;
        private readonly IEnumerable<ICommandQueryValidator<TCommandQuery, TResult>> _validators;

        /// <summary>
        /// Constructor to enable IoC decoration.
        /// </summary>
        /// <param name="commandQueryHandler">The next command query handler to call in the chain.</param>
        /// <param name="validators">The list of validation classes to execute for this type of command query.</param>
        public CommandQueryHandlerValidatorDecorator(
            ICommandQueryHandler<TCommandQuery, TResult> commandQueryHandler,
            ICollection<ICommandQueryValidator<TCommandQuery, TResult>> validators)
        {
            Guard.IsNotNull(commandQueryHandler, nameof(commandQueryHandler));
            Guard.IsNotNull(validators, nameof(validators));

            _commandQueryHandler = commandQueryHandler;
            _validators = validators;
        }

        /// <summary>
        /// Ensures the command query is in a valid state and then calls the next command query handler in the chain.
        /// </summary>
        /// <param name="commandQuery">The state of the command query.</param>
        /// <param name="token">
        /// A System.Threading.CancellationToken to observe while waiting for the task to complete.
        /// </param>
        /// <returns>An awaitable task.</returns>
        public async Task<IResult<TResult>> ExecuteAsync(TCommandQuery commandQuery,
            CancellationToken token = default)
        {
            List<BrokenRule> brokenRules = (await Task.WhenAll(_validators.AsParallel()
                    .Select(a => a.ValidateCommandQueryAsync(commandQuery, token)))
                .ConfigureAwait(false)).SelectMany(a => a).Where(a => a != null).ToList();

            if (brokenRules.Any())
            {
                return ResultHelper.ValidationError<TResult>(brokenRules);
            }

            return await _commandQueryHandler.ExecuteAsync(commandQuery, token).ConfigureAwait(false);
        }
    }
}