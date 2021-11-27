using MetalCore.CQS.Validation;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MetalCore.CQS.Command
{
    /// <summary>
    /// This interface defines the execution of a validation check for a given command.
    /// </summary>
    /// <typeparam name="TCommand">The type of command being handled.</typeparam>
    public interface ICommandValidator<in TCommand> where TCommand : ICommand
    {
        /// <summary>
        /// Verifies that the current command is valid.
        /// </summary>
        /// <param name="command">The command to validate.</param>
        /// <param name="token">
        /// A System.Threading.CancellationToken to observe while waiting for the task to complete.
        /// </param>
        /// <returns>An awaitable task.</returns>
        Task<IEnumerable<BrokenRule>> ValidateCommandAsync(
            TCommand command,
            CancellationToken token = default);
    }
}