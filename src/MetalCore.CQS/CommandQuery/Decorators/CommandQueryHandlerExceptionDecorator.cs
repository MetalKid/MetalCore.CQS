using MetalCore.CQS.Common.Results;
using MetalCore.CQS.Exceptions;
using MetalCore.CQS.Validation;
using System.Threading;
using System.Threading.Tasks;

namespace MetalCore.CQS.CommandQuery
{
    /// <summary>
    /// This class catches all exceptions that a command query handler produces and handles any logging.
    /// </summary>
    /// <typeparam name="TCommandQuery">The type of command query being handled.</typeparam>
    /// <typeparam name="TResult">The type of result being returned.</typeparam>
    public abstract class CommandQueryHandlerExceptionDecorator<TCommandQuery, TResult> : ICommandQueryHandler<TCommandQuery, TResult>
        where TCommandQuery : ICommandQuery<TResult>
    {
        private readonly ICommandQueryHandler<TCommandQuery, TResult> _commandQueryHandler;

        /// <summary>
        /// Constructor to enable IoC decoration.
        /// </summary>
        /// <param name="commandQueryHandler">The next command query handler to call in the chain.</param>
        /// <param name="logger">The logger to log any exceptions to.</param>
        /// <param name="userContext">Information about the currently logged in user.</param>
        /// <param name="resource">Translatable resources.</param>
        public CommandQueryHandlerExceptionDecorator(ICommandQueryHandler<TCommandQuery, TResult> commandQueryHandler)
        {
            Guard.IsNotNull(commandQueryHandler, nameof(commandQueryHandler));

            _commandQueryHandler = commandQueryHandler;
        }

        /// <summary>
        /// Catches/handles all exceptions and then calls the next command query handler in the chain
        /// if no exceptions are thrown.
        /// </summary>
        /// <param name="commandQuery">The state of the command query.</param>
        /// <param name="token">
        /// A System.Threading.CancellationToken to observe while waiting for the task to complete.
        /// </param>
        /// <returns>An awaitable task that returns the result of the command query.</returns>
        public virtual async Task<IResult<TResult>> ExecuteAsync(TCommandQuery commandQuery,
            CancellationToken token = default)
        {
            try
            {
                return await _commandQueryHandler.ExecuteAsync(commandQuery, token).ConfigureAwait(false);
            }
            catch (BrokenRuleException ex)
            {
                await HandleBrokenRuleExceptionAsync(commandQuery, ex).ConfigureAwait(false);
                return ResultHelper.ValidationError<TResult>(ex.BrokenRules);
            }
            catch (UserFriendlyException ex)
            {
                await HandleUserFriendlyExceptionAsync(commandQuery, ex).ConfigureAwait(false);
                return ResultHelper.Error<TResult>(ex.Message);
            }
            catch (DataNotFoundException ex)
            {
                await HandleDataNotFoundExceptionAsync(commandQuery, ex).ConfigureAwait(false);
                return ResultHelper.NoDataFoundError<TResult>();
            }
            catch (ConcurrencyException ex)
            {
                await HandleConcurrencyExceptionAsync(commandQuery, ex).ConfigureAwait(false);
                return ResultHelper.ConcurrencyError<TResult>();
            }
            catch (NoPermissionException ex)
            {
                await HandleNoPermissionExceptionAsync(commandQuery, ex).ConfigureAwait(false);
                return ResultHelper.NoPermissionError<TResult>();
            }
        }

        /// <summary>
        /// Handles a broken rule exception that was thrown.
        /// </summary>
        /// <param name="commandQuery">The state of the command query.</param>
        /// <param name="ex">The exception thrown.</param>
        /// <returns>An awaitable task.</returns>
        protected abstract Task HandleBrokenRuleExceptionAsync(TCommandQuery commandQuery, BrokenRuleException ex);

        /// <summary>
        /// Handles a user friendly exception that was thrown.
        /// </summary>
        /// <param name="commandQuery">The state of the command query.</param>
        /// <param name="ex">The exception thrown.</param>
        /// <returns>An awaitable task.</returns>
        protected abstract Task HandleUserFriendlyExceptionAsync(TCommandQuery commandQuery, UserFriendlyException ex);

        /// <summary>
        /// Handles a data not found exception that was thrown.
        /// </summary>
        /// <param name="commandQuery">The state of the command query.</param>
        /// <param name="ex">The exception thrown.</param>
        /// <returns>An awaitable task.</returns>
        protected abstract Task HandleDataNotFoundExceptionAsync(TCommandQuery commandQuery, DataNotFoundException ex);

        /// <summary>
        /// Handles a concurrency exception that was thrown.
        /// </summary>
        /// <param name="commandQuery">The state of the command query.</param>
        /// <param name="ex">The exception thrown.</param>
        /// <returns>An awaitable task.</returns>
        protected abstract Task HandleConcurrencyExceptionAsync(TCommandQuery commandQuery, ConcurrencyException ex);

        /// <summary>
        /// Handles a no permission exception that was thrown.
        /// </summary>
        /// <param name="commandQuery">The state of the command query.</param>
        /// <param name="ex">The exception thrown.</param>
        /// <returns>An awaitable task.</returns>
        protected abstract Task HandleNoPermissionExceptionAsync(TCommandQuery commandQuery, NoPermissionException ex);
    }
}