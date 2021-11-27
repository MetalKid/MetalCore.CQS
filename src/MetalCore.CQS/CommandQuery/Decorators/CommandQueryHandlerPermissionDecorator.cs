using MetalCore.CQS.Common.Results;
using MetalCore.CQS.Validation;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MetalCore.CQS.CommandQuery
{
    /// <summary>
    /// This class ensures the current user has permission to process this command query.
    /// </summary>
    /// <typeparam name="TCommandQuery">The type of command query being handled.</typeparam>
    /// <typeparam name="TResult">The type of result being returned.</typeparam>
    public class CommandQueryHandlerPermissionDecorator<TCommandQuery, TResult> : ICommandQueryHandler<TCommandQuery, TResult>
        where TCommandQuery : ICommandQuery<TResult>
    {
        private readonly ICommandQueryHandler<TCommandQuery, TResult> _commandQueryHandler;
        private readonly IEnumerable<ICommandQueryPermission<TCommandQuery, TResult>> _permissions;

        /// <summary>
        /// Constructor to enable IoC decoration.
        /// </summary>
        /// <param name="commandQueryHandler">The next command query handler to call in the chain.</param>
        /// <param name="permissions">The list of permission classes to execute for this type of command query.</param>
        public CommandQueryHandlerPermissionDecorator(
            ICommandQueryHandler<TCommandQuery, TResult> commandQueryHandler,
            ICollection<ICommandQueryPermission<TCommandQuery, TResult>> permissions)
        {
            Guard.IsNotNull(commandQueryHandler, nameof(commandQueryHandler));
            Guard.IsNotNull(permissions, nameof(permissions));

            _commandQueryHandler = commandQueryHandler;
            _permissions = permissions;
        }

        /// <summary>
        /// Ensures current user has permission to process this command query and 
        /// then calls the next command query handler in the chain.
        /// </summary>
        /// <param name="commandQuery">The state of the command query.</param>
        /// <param name="token">
        /// A System.Threading.CancellationToken to observe while waiting for the task to complete.
        /// </param>
        /// <returns>An awaitable task that returns the result of the command query.</returns>
        public async Task<IResult<TResult>> ExecuteAsync(TCommandQuery commandQuery, CancellationToken token = default)
        {
            bool[] result = await Task.WhenAll(_permissions.AsParallel().Select(a => a.HasPermissionAsync(commandQuery, token)))
                .ConfigureAwait(false);

            if (result.Any(hasPermission => !hasPermission))
            {
                return ResultHelper.NoPermissionError<TResult>();
            }

            return await _commandQueryHandler.ExecuteAsync(commandQuery, token).ConfigureAwait(false);
        }
    }
}