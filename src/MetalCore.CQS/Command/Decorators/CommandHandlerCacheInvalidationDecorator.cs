using MetalCore.CQS.Common.Results;
using MetalCore.CQS.Validation;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MetalCore.CQS.Command
{
    /// <summary>
    /// This class ensures the command invalidates all caches it could affect.
    /// </summary>
    /// <typeparam name="TCommand">The type of command being handled.</typeparam>
    public class CommandHandlerCacheInvalidationDecorator<TCommand> : ICommandHandler<TCommand> where TCommand : ICommand
    {
        private readonly ICommandHandler<TCommand> _commandHandler;
        private readonly IEnumerable<ICommandCacheInvalidation<TCommand>> _cacheInvalidators;

        /// <summary>
        /// Constructor to enable IoC decoration.
        /// </summary>
        /// <param name="commandHandler">The next command handler to call in the chain.</param>
        /// <param name="cacheInvalidators">The list of cache invalidator classes to execute for this type of command.</param>
        public CommandHandlerCacheInvalidationDecorator(
            ICommandHandler<TCommand> commandHandler,
            ICollection<ICommandCacheInvalidation<TCommand>> cacheInvalidators)
        {
            Guard.IsNotNull(commandHandler, nameof(commandHandler));
            Guard.IsNotNull(cacheInvalidators, nameof(cacheInvalidators));

            _commandHandler = commandHandler;
            _cacheInvalidators = cacheInvalidators;
        }

        /// <summary>
        /// Ensures the command invalidates all cahces it could affect and 
        /// then calls the next command handler in the chain.
        /// </summary>
        /// <param name="command">The state of the command.</param>
        /// <param name="token">
        /// A System.Threading.CancellationToken to observe while waiting for the task to complete.
        /// </param>
        /// <returns>An awaitable task with a command result.</returns>
        public async Task<IResult> ExecuteAsync(TCommand command, CancellationToken token = default)
        {
            IResult result = await _commandHandler.ExecuteAsync(command, token).ConfigureAwait(false);

            if (result == null || !result.IsSuccessful)
            {
                return result;
            }

            await Task.WhenAll(_cacheInvalidators.AsParallel()
                    .Select(a => a.InvalidateCacheAsync(command, token)))
                .ConfigureAwait(false);

            return result;
        }
    }
}