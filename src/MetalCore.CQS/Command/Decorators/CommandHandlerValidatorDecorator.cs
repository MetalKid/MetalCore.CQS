using MetalCore.CQS.Common.Results;
using MetalCore.CQS.Validation;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MetalCore.CQS.Command
{
    /// <summary>
    /// This class ensures the command is in a valid state to process.
    /// </summary>
    /// <typeparam name="TCommand">The type of command being handled.</typeparam>
    public class CommandHandlerValidatorDecorator<TCommand> : ICommandHandler<TCommand> where TCommand : ICommand
    {
        private readonly ICommandHandler<TCommand> _commandHandler;
        private readonly IEnumerable<ICommandValidator<TCommand>> _validators;

        /// <summary>
        /// Constructor to enable IoC decoration.
        /// </summary>
        /// <param name="commandHandler">The next command handler to call in the chain.</param>
        /// <param name="validate">The validation helper.</param>
        /// <param name="validators">The list of validation classes to execute for this type of command.</param>
        public CommandHandlerValidatorDecorator(
            ICommandHandler<TCommand> commandHandler,
            ICollection<ICommandValidator<TCommand>> validators)
        {
            Guard.IsNotNull(commandHandler, nameof(commandHandler));
            Guard.IsNotNull(validators, nameof(validators));

            _commandHandler = commandHandler;
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
        public async Task<IResult> ExecuteAsync(TCommand command, CancellationToken token = default)
        {
            List<BrokenRule> brokenRules = (await Task.WhenAll(_validators.AsParallel()
                    .Select(a => a.ValidateCommandAsync(command, token)))
                .ConfigureAwait(false)).SelectMany(a => a).Where(a => a != null).ToList();

            if (brokenRules.Any())
            {
                return ResultHelper.ValidationError(brokenRules);
            }

            return await _commandHandler.ExecuteAsync(command, token).ConfigureAwait(false);
        }
    }
}