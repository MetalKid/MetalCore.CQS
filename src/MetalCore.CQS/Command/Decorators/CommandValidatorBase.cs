using MetalCore.CQS.Validation;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MetalCore.CQS.Command
{
    /// <summary>
    /// This class handles running all the rules (sync and async) at the same time on multiple threads.
    /// </summary>
    /// <typeparam name="TCommand">The type of command being validated.</typeparam>
    public abstract class CommandValidatorBase<TCommand> : ValidatorBase<TCommand>,
        ICommandValidator<TCommand> where TCommand : ICommand
    {
        /// <summary>
        /// Gets the command being validated.
        /// </summary>
        protected TCommand Command { get; private set; }

        /// <summary>
        /// Ensures that the command is valid.
        /// </summary>
        /// <param name="command">The command to validate.</param>
        /// <param name="token">
        /// A System.Threading.CancellationToken to observe while waiting for the task to complete.
        /// </param>
        /// <returns>Any broken rules that were found.</returns>
        public async Task<IEnumerable<BrokenRule>> ValidateCommandAsync(
            TCommand command,
            CancellationToken token = default)
        {
            Guard.IsNotNull(command, nameof(command));

            Command = command;
            CreateRules(command, token);

            return await ValidateRulesAsync().ConfigureAwait(false);
        }
    }
}