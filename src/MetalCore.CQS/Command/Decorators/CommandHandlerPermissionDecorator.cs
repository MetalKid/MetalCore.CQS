using MetalCore.CQS.Common.Results;
using MetalCore.CQS.Validation;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MetalCore.CQS.Command
{
    /// <summary>
    /// This class ensures the current user has permission to process this command.
    /// </summary>
    /// <typeparam name="TCommand">The type of command being handled.</typeparam>
    public class CommandHandlerPermissionDecorator<TCommand> : ICommandHandler<TCommand> where TCommand : ICommand
    {
        private readonly ICommandHandler<TCommand> _commandHandler;
        private readonly IEnumerable<ICommandPermission<TCommand>> _permissions;

        /// <summary>
        /// Constructor to enable IoC decoration.
        /// </summary>
        /// <param name="commandHandler">The next command handler to call in the chain.</param>
        /// <param name="permissions">The list of permission classes to execute for this type of command.</param>
        public CommandHandlerPermissionDecorator(
            ICommandHandler<TCommand> commandHandler,
            ICollection<ICommandPermission<TCommand>> permissions)
        {
            Guard.IsNotNull(commandHandler, nameof(commandHandler));
            Guard.IsNotNull(permissions, nameof(permissions));

            _commandHandler = commandHandler;
            _permissions = permissions;
        }

        /// <summary>
        /// Ensures current user has permission to process this command and 
        /// then calls the next command handler in the chain.
        /// </summary>
        /// <param name="command">The state of the command.</param>
        /// <param name="token">
        /// A System.Threading.CancellationToken to observe while waiting for the task to complete.
        /// </param>
        /// <returns>An awaitable task with a command result.</returns>
        public async Task<IResult> ExecuteAsync(TCommand command, CancellationToken token = default)
        {
            bool[] result = await Task.WhenAll(_permissions.AsParallel().Select(a => a.HasPermissionAsync(command, token)))
                .ConfigureAwait(false);

            if (result.Any(hasPermission => !hasPermission))
            {
                return ResultHelper.NoPermissionError();
            }

            return await _commandHandler.ExecuteAsync(command, token).ConfigureAwait(false);
        }
    }
}