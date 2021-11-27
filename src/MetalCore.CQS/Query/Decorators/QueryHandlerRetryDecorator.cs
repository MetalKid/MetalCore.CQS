using MetalCore.CQS.Common;
using MetalCore.CQS.Common.Results;
using MetalCore.CQS.Exceptions;
using MetalCore.CQS.Helpers;
using MetalCore.CQS.Validation;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MetalCore.CQS.Query
{
    /// <summary>
    /// This class will attempt to retry a command query the configured number of times if the error could be temporary.
    /// </summary>
    /// <typeparam name="TQuery">The type of command query being handled.</typeparam>
    /// <typeparam name="TResult">The type of result being returned.</typeparam>
    public class QueryHandlerRetryDecorator<TQuery, TResult> :
        IQueryHandler<TQuery, TResult> where TQuery : IQuery<TResult>
    {
        private readonly IQueryHandler<TQuery, TResult> _queryHandler;
        private int _maxRetries;
        private int _retryDelayMilliseconds;

        /// <summary>
        /// Constructor to enable IoC decoration.
        /// </summary>
        /// <param name="queryHandler">The next command query handler to call in the chain.</param>
        public QueryHandlerRetryDecorator(IQueryHandler<TQuery, TResult> queryHandler)
        {
            Guard.IsNotNull(queryHandler, nameof(queryHandler));

            _queryHandler = queryHandler;
        }

        /// <summary>
        /// Calls the next command query handler in the chain and attempts retries if configured.
        /// </summary>
        /// <param name="query">The state of the command query.</param>
        /// <param name="token">
        /// A System.Threading.CancellationToken to observe while waiting for the task to complete.
        /// </param>
        /// <returns>An awaitable task.</returns>
        public async Task<IResult<TResult>> ExecuteAsync(TQuery query,
            CancellationToken token = default)
        {
            if (!(query is ICqsRetry retry))
            {
                return await _queryHandler.ExecuteAsync(query, token).ConfigureAwait(false);
            }

            _maxRetries = retry.MaxRetries;
            _retryDelayMilliseconds = retry.RetryDelayMilliseconds;
            return await ExecuteWithRetryAsync(query, 0, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Calls the next command query handler in the chain and attempts retries if configured.
        /// </summary>
        /// <param name="query">The state of the command query.</param>
        /// <param name="tries">The total number of tries this has attempted to run.</param>
        /// <param name="token">
        /// A System.Threading.CancellationToken to observe while waiting for the task to complete.
        /// </param>
        /// <returns>An awaitable task.</returns>
        private async Task<IResult<TResult>> ExecuteWithRetryAsync(TQuery query, int tries,
            CancellationToken token = default)
        {
            try
            {
                return await _queryHandler.ExecuteAsync(query, token).ConfigureAwait(false);
            }
            catch (BrokenRuleException)
            {
                throw; // Cannot retry this
            }
            catch (NoPermissionException)
            {
                throw; // Cannot retry this
            }
            catch (DataNotFoundException)
            {
                throw; // Cannot retry this
            }
            catch (ConcurrencyException)
            {
                throw; // Cannot retry this
            }
            catch (Exception ex)
            {
                if (tries >= _maxRetries)
                {
                    throw;
                }

                if (query is ICqsRetrySpecific specific)
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
                return await ExecuteWithRetryAsync(query, ++tries, token).ConfigureAwait(false);
            }
        }
    }
}