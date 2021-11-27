using MetalCore.CQS.Common.Results;
using System.Threading;
using System.Threading.Tasks;

namespace MetalCore.CQS.Command
{
    /// <summary>
    /// This interface defines the execution of a command handler for a given command.
    /// </summary>
    /// <typeparam name="TCommand">The type of command being handled.</typeparam>
    public interface ICommandHandler<in TCommand> where TCommand : ICommand
    {
        /// <summary>
        /// Executes the command handler logic for the given command.
        /// </summary>
        /// <param name="command">The state of the command.</param>
        /// <param name="token">
        /// A System.Threading.CancellationToken to observe while waiting for the task to complete.
        /// </param>
        /// <returns>An awaitable task with a command result.</returns>
        Task<IResult> ExecuteAsync(TCommand command, CancellationToken token = default);
    }
}