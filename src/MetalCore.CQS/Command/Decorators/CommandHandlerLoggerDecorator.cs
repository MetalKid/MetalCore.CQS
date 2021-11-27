using MetalCore.CQS.Common.Results;
using MetalCore.CQS.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MetalCore.CQS.Command
{
    /// <summary>
    /// This decorator allows custom logging at start, end, and errors while processing the given command.
    /// </summary>
    /// <typeparam name="TCommand">The type of command being run.</typeparam>
    public class CommandHandlerLoggerDecorator<TCommand> : ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        private readonly ICommandHandler<TCommand> _commandHandler;
        private readonly IEnumerable<ICommandLogger<TCommand>> _commandLoggers;

        /// <summary>
        /// Constructor to enable IoC decoration.
        /// </summary>
        /// <param name="commandHandler">The next command handler to call in the chain.</param>
        /// <param name="commandLoggers">Optional loggers to log specific information for a command.</param>
        public CommandHandlerLoggerDecorator(
            ICommandHandler<TCommand> commandHandler,
            ICollection<ICommandLogger<TCommand>> commandLoggers)
        {
            Guard.IsNotNull(commandHandler, nameof(commandHandler));

            _commandHandler = commandHandler;
            _commandLoggers = commandLoggers;
        }

        /// <summary>
        /// Calls start/end/error log methods while running a specific command.
        /// </summary>
        /// <param name="command">The state of the command.</param>
        /// <param name="token">
        /// A System.Threading.CancellationToken to observe while waiting for the task to complete.
        /// </param>
        /// <returns>An awaitable task that returns the result of the command.</returns>
        public async Task<IResult> ExecuteAsync(TCommand command,
            CancellationToken token = default)
        {
            try
            {
                if (_commandLoggers != null)
                {
                    await Task.WhenAll(_commandLoggers.AsParallel()
                        .Select(a => a.LogStartAsync(command, token))).ConfigureAwait(false);
                }

                IResult result = await _commandHandler.ExecuteAsync(command, token).ConfigureAwait(false);

                if (_commandLoggers != null)
                {
                    await Task.WhenAll(_commandLoggers.AsParallel()
                        .Select(a => a.LogEndAsync(command, result, token))).ConfigureAwait(false);
                }

                return result;
            }
            catch (Exception ex)
            {
                await Task.WhenAll(_commandLoggers.AsParallel()
                    .Select(a => a.LogErrorAsync(command, ex, token))).ConfigureAwait(false);

                throw;
            }
        }
    }
}