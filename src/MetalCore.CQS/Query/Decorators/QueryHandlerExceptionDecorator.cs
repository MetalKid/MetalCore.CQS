using MetalCore.CQS.Common.Results;
using MetalCore.CQS.Exceptions;
using MetalCore.CQS.Validation;
using System.Threading;
using System.Threading.Tasks;

namespace MetalCore.CQS.Query
{
    /// <summary>
    /// This class catches all exceptions that a query handler produces and handles any logging.
    /// </summary>
    /// <typeparam name="TQuery">The type of query being handled.</typeparam>
    /// <typeparam name="TResult">The type of result being returned.</typeparam>
    public abstract class QueryHandlerExceptionDecorator<TQuery, TResult> : IQueryHandler<TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        private readonly IQueryHandler<TQuery, TResult> _queryHandler;

        /// <summary>
        /// Constructor to enable IoC decoration.
        /// </summary>
        /// <param name="queryHandler">The next query handler to call in the chain.</param>
        public QueryHandlerExceptionDecorator(IQueryHandler<TQuery, TResult> queryHandler)
        {
            Guard.IsNotNull(queryHandler, nameof(queryHandler));

            _queryHandler = queryHandler;
        }

        /// <summary>
        /// Catches/handles all exceptions and then calls the next query handler in the chain.
        /// </summary>
        /// <param name="query">The state of the query.</param>
        /// <param name="token">
        /// A System.Threading.CancellationToken to observe while waiting for the task to complete.
        /// </param>
        /// <returns>An awaitable task that returns the result of the query.</returns>
        public virtual async Task<IResult<TResult>> ExecuteAsync(TQuery query,
            CancellationToken token = default)
        {
            try
            {
                return await _queryHandler.ExecuteAsync(query, token).ConfigureAwait(false);
            }
            catch (BrokenRuleException ex)
            {
                await HandleBrokenRuleExceptionAsync(query, ex).ConfigureAwait(false);
                return ResultHelper.ValidationError<TResult>(ex.BrokenRules);
            }
            catch (UserFriendlyException ex)
            {
                await HandleUserFriendlyExceptionAsync(query, ex).ConfigureAwait(false);
                return ResultHelper.Error<TResult>(ex.Message);
            }
            catch (DataNotFoundException ex)
            {
                await HandleDataNotFoundExceptionAsync(query, ex).ConfigureAwait(false);
                return ResultHelper.NoDataFoundError<TResult>();
            }
            catch (ConcurrencyException ex)
            {
                await HandleConcurrencyExceptionAsync(query, ex).ConfigureAwait(false);
                return ResultHelper.ConcurrencyError<TResult>();
            }
            catch (NoPermissionException ex)
            {
                await HandleNoPermissionExceptionAsync(query, ex).ConfigureAwait(false);
                return ResultHelper.NoPermissionError<TResult>();
            }
        }

        /// <summary>
        /// Handles a broken rule exception that was thrown.
        /// </summary>
        /// <param name="query">The state of the query.</param>
        /// <param name="ex">The exception thrown.</param>
        /// <returns>An awaitable task.</returns>
        protected abstract Task HandleBrokenRuleExceptionAsync(TQuery query, BrokenRuleException ex);

        /// <summary>
        /// Handles a user friendly exception that was thrown.
        /// </summary>
        /// <param name="query">The state of the query.</param>
        /// <param name="ex">The exception thrown.</param>
        /// <returns>An awaitable task.</returns>
        protected abstract Task HandleUserFriendlyExceptionAsync(TQuery query, UserFriendlyException ex);

        /// <summary>
        /// Handles a data not found exception that was thrown.
        /// </summary>
        /// <param name="query">The state of the query.</param>
        /// <param name="ex">The exception thrown.</param>
        /// <returns>An awaitable task.</returns>
        protected abstract Task HandleDataNotFoundExceptionAsync(TQuery query, DataNotFoundException ex);

        /// <summary>
        /// Handles a concurrency exception that was thrown.
        /// </summary>
        /// <param name="query">The state of the query.</param>
        /// <param name="ex">The exception thrown.</param>
        /// <returns>An awaitable task.</returns>
        protected abstract Task HandleConcurrencyExceptionAsync(TQuery query, ConcurrencyException ex);

        /// <summary>
        /// Handles a no permission exception that was thrown.
        /// </summary>
        /// <param name="query">The state of the query.</param>
        /// <param name="ex">The exception thrown.</param>
        /// <returns>An awaitable task.</returns>
        protected abstract Task HandleNoPermissionExceptionAsync(TQuery query, NoPermissionException ex);
    }
}