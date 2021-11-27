using MetalCore.CQS.Common;
using MetalCore.CQS.Common.Results;
using MetalCore.CQS.Exceptions;
using MetalCore.CQS.Helpers;
using MetalCore.CQS.Validation;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MetalCore.CQS.CommandQuery
{
    /// <summary>
    /// This class will attempt to retry a command query the configured number of times if the error could be temporary.
    /// </summary>
    /// <typeparam name="TCommandQuery">The type of command query being handled.</typeparam>
    /// <typeparam name="TResult">The type of result being returned.</typeparam>
    public class CommandQueryHandlerRetryDecorator<TCommandQuery, TResult> :
        ICommandQueryHandler<TCommandQuery, TResult> where TCommandQuery : ICommandQuery<TResult>
    {
        private readonly ICommandQueryHandler<TCommandQuery, TResult> _commandQueryHandler;
        private int _maxRetries;
        private int _retryDelayMilliseconds;

        /// <summary>
        /// Constructor to enable IoC decoration.
        /// </summary>
        /// <param name="commandQueryHandler">The next command query handler to call in the chain.</param>
        public CommandQueryHandlerRetryDecorator(ICommandQueryHandler<TCommandQuery, TResult> commandQueryHandler)
        {
            Guard.IsNotNull(commandQueryHandler, nameof(commandQueryHandler));

            _commandQueryHandler = commandQueryHandler;
        }

        /// <summary>
        /// Calls the next command query handler in the chain and attempts retries if configured.
        /// </summary>
        /// <param name="commandQuery">The state of the command query.</param>
        /// <param name="token">
        /// A System.Threading.CancellationToken to observe while waiting for the task to complete.
        /// </param>
        /// <returns>An awaitable task.</returns>
        public async Task<IResult<TResult>> ExecuteAsync(TCommandQuery commandQuery,
            CancellationToken token = default)
        {
            if (!(commandQuery is ICqsRetry retry))
            {
                return await _commandQueryHandler.ExecuteAsync(commandQuery, token).ConfigureAwait(false);
            }

            _maxRetries = retry.MaxRetries;
            _retryDelayMilliseconds = retry.RetryDelayMilliseconds;
            return await ExecuteWithRetryAsync(commandQuery, 0, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Calls the next command query handler in the chain and attempts retries if configured.
        /// </summary>
        /// <param name="commandQuery">The state of the command query.</param>
        /// <param name="tries">The total number of tries this has attempted to run.</param>
        /// <param name="token">
        /// A System.Threading.CancellationToken to observe while waiting for the task to complete.
        /// </param>
        /// <returns>An awaitable task.</returns>
        private async Task<IResult<TResult>> ExecuteWithRetryAsync(TCommandQuery commandQuery, int tries,
            CancellationToken token = default)
        {
            try
            {
                return await _commandQueryHandler.ExecuteAsync(commandQuery, token).ConfigureAwait(false);
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

                if (commandQuery is ICqsRetrySpecific specific)
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

                return await ExecuteWithRetryAsync(commandQuery, ++tries, token).ConfigureAwait(false);
            }
        }
    }
}