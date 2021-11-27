using MetalCore.CQS.Common.Results;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MetalCore.CQS.Command
{
    /// <summary>
    /// This interface allows a specific command to do specific, custom logging.
    /// </summary>
    /// <typeparam name="TCommand">The type of command being run.</typeparam>
    public interface ICommandLogger<in TCommand> where TCommand : ICommand
    {
        /// <summary>
        /// Logs information at the start of processing a command.
        /// </summary>
        /// <param name="command">The command to log information for.</param>
        /// <param name="token">
        /// A System.Threading.CancellationToken to observe while waiting for the task to complete.
        /// </param>
        /// <returns>An awaitable task that returns the result of the command.</returns>
        Task LogStartAsync(
            TCommand command,
            CancellationToken token = default);

        /// <summary>
        /// Logs information when an exception occurrs during the processing of a command.
        /// </summary>
        /// <param name="command">The command to log information for.</param>
        /// <param name="ex">The exception that occurred while processing the command.</param>
        /// <param name="token">
        /// A System.Threading.CancellationToken to observe while waiting for the task to complete.
        /// </param>
        /// <returns>An awaitable task that returns the result of the command.</returns>
        Task LogErrorAsync(
            TCommand command,
            Exception ex,
            CancellationToken token = default);

        /// <summary>
        /// Logs information at the end of processing a command.
        /// </summary>
        /// <param name="command">The command to log information for.</param>
        /// <param name="result">The result data of the command.</param>
        /// <param name="token">
        /// A System.Threading.CancellationToken to observe while waiting for the task to complete.
        /// </param>
        /// <returns>An awaitable task that returns the result of the command.</returns>
        Task LogEndAsync(
            TCommand command,
            IResult result,
            CancellationToken token = default);
    }
}