using MetalCore.CQS.Common;
using MetalCore.CQS.Common.Results;
using MetalCore.CQS.Validation;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace MetalCore.CQS.Command
{
    /// <summary>
    /// This decorator class is used to keep track of how long commands take to run.
    /// <typeparam name="TCommand">The type of command to time.</typeparam>
    public abstract class CommandHandlerTimingDecorator<TCommand> :
        ICommandHandler<TCommand> where TCommand : ICommand
    {
        private readonly ICommandHandler<TCommand> _commandHandler;

        /// <summary>
        /// Constructor to enable IoC decoration.
        /// </summary>
        /// <param name="commandHandler">The next command handler to call in the chain.</param>
        public CommandHandlerTimingDecorator(ICommandHandler<TCommand> commandHandler)
        {
            Guard.IsNotNull(commandHandler, nameof(commandHandler));

            _commandHandler = commandHandler;
        }

        /// <summary>
        /// Logs how long the command takes to run.
        /// </summary>
        /// <param name="command">The state of the command.</param>
        /// <param name="token">
        /// A System.Threading.CancellationToken to observe while waiting for the task to complete.
        /// </param>
        /// <returns>An awaitable task with a command result.</returns>
        public async Task<IResult> ExecuteAsync(TCommand command,
            CancellationToken token = default)
        {
            IResult result = null;
            Stopwatch timer = Stopwatch.StartNew();
            try
            {
                result = await _commandHandler.ExecuteAsync(command, token).ConfigureAwait(false);
            }
            finally
            {
                timer.Stop();

                await HandleTimerEndAsync(command, result, timer.ElapsedMilliseconds).ConfigureAwait(false);

                if (command is ICqsTimingWarningThreshold warningThreshold &&
                    timer.ElapsedMilliseconds >= warningThreshold.WarningThresholdMilliseconds)
                {
                    await HandleTimerWarningAsync(command, result, timer.ElapsedMilliseconds, warningThreshold)
                        .ConfigureAwait(false);
                }
            }

            return result;
        }

        /// <summary>
        /// This method is called after the command has finishes running.
        /// </summary>
        /// <param name="command">The state of the command.</param>
        /// <param name="result">The result of the command running.</param>
        /// <param name="totalMilliseconds">The total number of milliseconds it took to run the commnad.</param>
        /// <returns>An awaitable task.</returns>
        protected abstract Task HandleTimerEndAsync(TCommand command, IResult result, long totalMilliseconds);

        /// <summary>
        /// This method is called after the command has finishes running and it took longer than it should have.
        /// </summary>
        /// <param name="command">The state of the command.</param>
        /// <param name="result">The result of the command running.</param>
        /// <param name="totalMilliseconds">The total number of milliseconds it took to run the commnad.</param>
        /// <param name="warningThreshold">The data about the warning.</param>
        /// <returns>An awaitable task.</returns>
        protected abstract Task HandleTimerWarningAsync(
            TCommand command,
            IResult result,
            long totalMilliseconds,
            ICqsTimingWarningThreshold warningThreshold);
    }
}