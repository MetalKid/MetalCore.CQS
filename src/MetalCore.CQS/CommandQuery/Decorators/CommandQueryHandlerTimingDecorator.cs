using MetalCore.CQS.Common;
using MetalCore.CQS.Common.Results;
using MetalCore.CQS.Validation;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace MetalCore.CQS.CommandQuery
{
    /// <summary>
    /// This decorator class is used to keep track of how long command queries take to run.
    /// </summary>
    /// <typeparam name="TCommandQuery">The type of command query to time.</typeparam>
    /// <typeparam name="TResult">The type of result being returned.</typeparam>
    public abstract class CommandQueryHandlerTimingDecorator<TCommandQuery, TResult> :
        ICommandQueryHandler<TCommandQuery, TResult> where TCommandQuery : ICommandQuery<TResult>
    {
        private readonly ICommandQueryHandler<TCommandQuery, TResult> _commandQueryHandler;

        /// <summary>
        /// Constructor to enable IoC decoration.
        /// </summary>
        /// <param name="commandQueryHandler">The next command query handler to call in the chain.</param>
        public CommandQueryHandlerTimingDecorator(
            ICommandQueryHandler<TCommandQuery, TResult> commandQueryHandler)
        {
            Guard.IsNotNull(commandQueryHandler, nameof(commandQueryHandler));

            _commandQueryHandler = commandQueryHandler;
        }

        /// <summary>
        /// Logs how long the command query takes to run.
        /// </summary>
        /// <param name="commandQuery">The state of the command query.</param>
        /// <param name="token">
        /// A System.Threading.CancellationToken to observe while waiting for the task to complete.
        /// </param>
        /// <returns>An awaitable task with a command result.</returns>
        public async Task<IResult<TResult>> ExecuteAsync(TCommandQuery commandQuery,
            CancellationToken token = default)
        {
            IResult<TResult> result = null;
            Stopwatch timer = Stopwatch.StartNew();
            try
            {
                result = await _commandQueryHandler.ExecuteAsync(commandQuery, token).ConfigureAwait(false);
            }
            finally
            {
                timer.Stop();

                await HandleTimerEndAsync(commandQuery, result, timer.ElapsedMilliseconds).ConfigureAwait(false);

                if (commandQuery is ICqsTimingWarningThreshold warningThreshold &&
                    timer.ElapsedMilliseconds >= warningThreshold.WarningThresholdMilliseconds)
                {
                    await HandleTimerWarningAsync(commandQuery, result, timer.ElapsedMilliseconds, warningThreshold)
                        .ConfigureAwait(false);
                }
            }

            return result;
        }

        /// <summary>
        /// This method is called after the command has finishes running.
        /// </summary>
        /// <param name="commandQuery">The state of the command.</param>
        /// <param name="result">The result of the command running.</param>
        /// <param name="totalMilliseconds">The total number of milliseconds it took to run the commnad.</param>
        /// <returns>An awaitable task.</returns>
        protected abstract Task HandleTimerEndAsync(TCommandQuery commandQuery, IResult result, long totalMilliseconds);

        /// <summary>
        /// This method is called after the command has finishes running and it took longer than it should have.
        /// </summary>
        /// <param name="commandQuery">The state of the command.</param>
        /// <param name="result">The result of the command running.</param>
        /// <param name="totalMilliseconds">The total number of milliseconds it took to run the commnad.</param>
        /// <param name="warningThreshold">The data about the warning.</param>
        /// <returns>An awaitable task.</returns>
        protected abstract Task HandleTimerWarningAsync(
            TCommandQuery commandQuery,
            IResult result,
            long totalMilliseconds,
            ICqsTimingWarningThreshold warningThreshold);
    }
}