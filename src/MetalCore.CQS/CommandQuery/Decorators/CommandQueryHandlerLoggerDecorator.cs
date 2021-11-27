using MetalCore.CQS.Common.Results;
using MetalCore.CQS.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MetalCore.CQS.CommandQuery
{
    /// <summary>
    /// This decorator allows custom logging at start, end, and errors while processing the given command query.
    /// </summary>
    /// <typeparam name="TCommandQuery">The type of command query being run.</typeparam>
    /// <typeparam name="TResult">The type of result the command query returns.</typeparam>
    public class CommandQueryHandlerLoggerDecorator<TCommandQuery, TResult> : ICommandQueryHandler<TCommandQuery, TResult>
        where TCommandQuery : ICommandQuery<TResult>
    {
        private readonly ICommandQueryHandler<TCommandQuery, TResult> _commandQueryHandler;
        private readonly IEnumerable<ICommandQueryLogger<TCommandQuery, TResult>> _commandQueryLoggers;

        /// <summary>
        /// Constructor to enable IoC decoration.
        /// </summary>
        /// <param name="commandQueryHandler">The next command query handler to call in the chain.</param>
        /// <param name="commandQueryLoggers">Optional loggers to log specific information for a command query.</param>
        public CommandQueryHandlerLoggerDecorator(
            ICommandQueryHandler<TCommandQuery, TResult> commandQueryHandler,
            ICollection<ICommandQueryLogger<TCommandQuery, TResult>> commandQueryLoggers)
        {
            Guard.IsNotNull(commandQueryHandler, nameof(commandQueryHandler));

            _commandQueryHandler = commandQueryHandler;
            _commandQueryLoggers = commandQueryLoggers;
        }

        /// <summary>
        /// Calls stat/end/error log methods while running a specific command query.
        /// </summary>
        /// <param name="commandQuery">The state of the command query.</param>
        /// <param name="token">
        /// A System.Threading.CancellationToken to observe while waiting for the task to complete.
        /// </param>
        /// <returns>An awaitable task that returns the result of the command query.</returns>
        public async Task<IResult<TResult>> ExecuteAsync(TCommandQuery commandQuery,
            CancellationToken token = default)
        {
            try
            {
                if (_commandQueryLoggers != null)
                {
                    await Task.WhenAll(_commandQueryLoggers.AsParallel()
                    .Select(a => a.LogStartAsync(commandQuery, token))).ConfigureAwait(false);
                }

                IResult<TResult> result = await _commandQueryHandler.ExecuteAsync(commandQuery, token).ConfigureAwait(false);

                if (_commandQueryLoggers != null)
                {
                    await Task.WhenAll(_commandQueryLoggers.AsParallel()
                    .Select(a => a.LogEndAsync(commandQuery, result, token))).ConfigureAwait(false);
                }

                return result;
            }
            catch (Exception ex)
            {
                await Task.WhenAll(_commandQueryLoggers.AsParallel()
                    .Select(a => a.LogErrorAsync(commandQuery, ex, token))).ConfigureAwait(false);

                throw;
            }
        }
    }
}