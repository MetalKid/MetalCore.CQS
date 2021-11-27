using MetalCore.CQS.Common;
using MetalCore.CQS.Common.Results;
using MetalCore.CQS.Exceptions;
using MetalCore.CQS.Helpers;
using MetalCore.CQS.Validation;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MetalCore.CQS.Command
{
    /// <summary>
    /// This class will attempt to retry a command the configured number of times if the error could be temporary.
    /// </summary>
    /// <typeparam name="TCommand">The type of command being handled.</typeparam>
    public class CommandHandlerRetryDecorator<TCommand> : ICommandHandler<TCommand> where TCommand : ICommand
    {
        private readonly ICommandHandler<TCommand> _commandHandler;
        private int _maxRetries;
        private int _retryDelayMilliseconds;

        /// <summary>
        /// Constructor to enable IoC decoration.
        /// </summary>
        /// <param name="commandHandler">The next command handler to call in the chain.</param>
        public CommandHandlerRetryDecorator(ICommandHandler<TCommand> commandHandler)
        {
            Guard.IsNotNull(commandHandler, nameof(commandHandler));

            _commandHandler = commandHandler;
        }

        /// <summary>
        /// Calls the next command handler in the chain and attempts retries if configured.
        /// </summary>
        /// <param name="command">The state of the command.</param>
        /// <param name="token">
        /// A System.Threading.CancellationToken to observe while waiting for the task to complete.
        /// </param>
        /// <returns>An awaitable task with a command result.</returns>
        public async Task<IResult> ExecuteAsync(TCommand command, CancellationToken token = default)
        {
            if (!(command is ICqsRetry retry))
            {
                return await _commandHandler.ExecuteAsync(command, token).ConfigureAwait(false);
            }

            _maxRetries = retry.MaxRetries;
            _retryDelayMilliseconds = retry.RetryDelayMilliseconds;
            return await ExecuteWithRetryAsync(command, 0, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Calls the next command handler in the chain and attempts retries if configured.
        /// </summary>
        /// <param name="command">The state of the command.</param>
        /// <param name="tries">The total number of tries this has attempted to run.</param>
        /// <param name="token">
        /// A System.Threading.CancellationToken to observe while waiting for the task to complete.
        /// </param>
        /// <returns>An awaitable task.</returns>
        private async Task<IResult> ExecuteWithRetryAsync(TCommand command, int tries,
            CancellationToken token = default)
        {
            try
            {
                return await _commandHandler.ExecuteAsync(command, token).ConfigureAwait(false);
            }
            catch (BrokenRuleException)
            {
                throw; // Cannot retry this
            }
            catch (NoPermissionException)
            {
                throw; // Cannot retry this
            }
            catch (ConcurrencyException)
            {
                throw; // Cannot retry this
            }
            catch (DataNotFoundException)
            {
                throw; // Cannot retry this
            }
            catch (Exception ex)
            {
                if (tries >= _maxRetries)
                {
                    throw;
                }

                if (command is ICqsRetrySpecific specific)
                {
                    if (!RetryHelper.HasAnyExceptionMatch(
                        specific.RetryCheckBaseTypes,
                        specific.RetryCheckInnerExceptions,
                        ex,
                        specific.OnlyRetryForExceptionsOfTheseTypes))
                    {
                        throw;
                    }
                }

                if (_retryDelayMilliseconds > 0)
                {
                    await Task.Delay(_retryDelayMilliseconds, token).ConfigureAwait(false);
                }

                return await ExecuteWithRetryAsync(command, ++tries, token).ConfigureAwait(false);
            }
        }
    }
}