using MetalCore.CQS.Common.Results;
using MetalCore.CQS.Exceptions;
using MetalCore.CQS.Validation;
using System.Threading;
using System.Threading.Tasks;

namespace MetalCore.CQS.Command
{
    /// <summary>
    /// This class catches all exceptions that a command handler produces and handles any logging.
    /// </summary>
    /// <typeparam name="TCommand">The type of command being handled.</typeparam>
    public abstract class CommandHandlerExceptionDecorator<TCommand> : ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        private readonly ICommandHandler<TCommand> _commandHandler;

        /// <summary>
        /// Constructor to enable IoC decoration.
        /// </summary>
        /// <param name="commandHandler">The next command handler to call in the chain.</param>
        public CommandHandlerExceptionDecorator(ICommandHandler<TCommand> commandHandler)
        {
            Guard.IsNotNull(commandHandler, nameof(commandHandler));

            _commandHandler = commandHandler;
        }

        /// <summary>
        /// Catches/handles all exceptions and then calls the next command handler in the chain.
        /// </summary>
        /// <param name="command">The state of the command.</param>
        /// <param name="token">
        /// A System.Threading.CancellationToken to observe while waiting for the task to complete.
        /// </param>
        /// <returns>An awaitable task with a command result.</returns>
        public virtual async Task<IResult> ExecuteAsync(TCommand command, CancellationToken token = default)
        {
            try
            {
                return await _commandHandler.ExecuteAsync(command, token).ConfigureAwait(false);
            }
            catch (BrokenRuleException ex)
            {
                await HandleBrokenRuleExceptionAsync(command, ex).ConfigureAwait(false);
                return ResultHelper.ValidationError(ex.BrokenRules);
            }
            catch (UserFriendlyException ex)
            {
                await HandleUserFriendlyExceptionAsync(command, ex).ConfigureAwait(false);
                return ResultHelper.Error(ex.Message);
            }
            catch (DataNotFoundException ex)
            {
                await HandleDataNotFoundExceptionAsync(command, ex).ConfigureAwait(false);
                return ResultHelper.NoDataFoundError();
            }
            catch (ConcurrencyException ex)
            {
                await HandleConcurrencyExceptionAsync(command, ex).ConfigureAwait(false);
                return ResultHelper.ConcurrencyError();
            }
            catch (NoPermissionException ex)
            {
                await HandleNoPermissionExceptionAsync(command, ex).ConfigureAwait(false);
                return ResultHelper.NoPermissionError();
            }
        }

        /// <summary>
        /// Handles a broken rule exception that was thrown.
        /// </summary>
        /// <param name="command">The state of the command.</param>
        /// <param name="ex">The exception thrown.</param>
        /// <returns>An awaitable task.</returns>
        protected abstract Task HandleBrokenRuleExceptionAsync(TCommand command, BrokenRuleException ex);

        /// <summary>
        /// Handles a user friendly exception that was thrown.
        /// </summary>
        /// <param name="command">The state of the command.</param>
        /// <param name="ex">The exception thrown.</param>
        /// <returns>An awaitable task.</returns>
        protected abstract Task HandleUserFriendlyExceptionAsync(TCommand command, UserFriendlyException ex);

        /// <summary>
        /// Handles a data not found exception that was thrown.
        /// </summary>
        /// <param name="command">The state of the command.</param>
        /// <param name="ex">The exception thrown.</param>
        /// <returns>An awaitable task.</returns>
        protected abstract Task HandleDataNotFoundExceptionAsync(TCommand command, DataNotFoundException ex);

        /// <summary>
        /// Handles a concurrency exception that was thrown.
        /// </summary>
        /// <param name="command">The state of the command.</param>
        /// <param name="ex">The exception thrown.</param>
        /// <returns>An awaitable task.</returns>
        protected abstract Task HandleConcurrencyExceptionAsync(TCommand command, ConcurrencyException ex);

        /// <summary>
        /// Handles a no permission exception that was thrown.
        /// </summary>
        /// <param name="command">The state of the command.</param>
        /// <param name="ex">The exception thrown.</param>
        /// <returns>An awaitable task.</returns>
        protected abstract Task HandleNoPermissionExceptionAsync(TCommand command, NoPermissionException ex);
    }
}