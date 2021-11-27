using System.Threading;
using System.Threading.Tasks;

namespace MetalCore.CQS.Command
{
    /// <summary>
    /// This interface defines the execution of a permission check for a given command.
    /// </summary>
    /// <typeparam name="TCommand">The type of command being handled.</typeparam>
    public interface ICommandPermission<in TCommand> where TCommand : ICommand
    {
        /// <summary>
        /// Verifies that the current user has permission to process this command.
        /// </summary>
        /// <param name="command">The command to validate.</param>
        /// <param name="token">
        /// A System.Threading.CancellationToken to observe while waiting for the task to complete.
        /// </param>
        /// <returns>An awaitable task whether the user has permission to run this command.</returns>
        Task<bool> HasPermissionAsync(TCommand command, CancellationToken token = default);
    }
}