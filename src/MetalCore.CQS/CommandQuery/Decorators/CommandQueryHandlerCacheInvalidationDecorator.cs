using MetalCore.CQS.Common.Results;
using MetalCore.CQS.Validation;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MetalCore.CQS.CommandQuery
{
    /// <summary>
    /// This class ensures the command query invalidates all caches it could affect.
    /// </summary>
    /// <typeparam name="TCommandQuery">The type of command query being handled.</typeparam>
    /// <typeparam name="TResult">The type of result being returned.</typeparam>
    public class CommandQueryHandlerCacheInvalidationDecorator<TCommandQuery, TResult> :
        ICommandQueryHandler<TCommandQuery, TResult> where TCommandQuery : ICommandQuery<TResult>
    {
        private readonly ICommandQueryHandler<TCommandQuery, TResult> _commandQueryHandler;
        private readonly IEnumerable<ICommandQueryCacheInvalidation<TCommandQuery, TResult>> _cacheInvalidators;

        /// <summary>
        /// Constructor to enable IoC decoration.
        /// </summary>
        /// <param name="commandQueryHandler">The next command query handler to call in the chain.</param>
        /// <param name="cacheInvalidators">The list of cache invalidator classes to execute for this type of command query.</param>
        public CommandQueryHandlerCacheInvalidationDecorator(
            ICommandQueryHandler<TCommandQuery, TResult> commandQueryHandler,
            ICollection<ICommandQueryCacheInvalidation<TCommandQuery, TResult>> cacheInvalidators)
        {
            Guard.IsNotNull(commandQueryHandler, nameof(commandQueryHandler));
            Guard.IsNotNull(cacheInvalidators, nameof(cacheInvalidators));

            _commandQueryHandler = commandQueryHandler;
            _cacheInvalidators = cacheInvalidators;
        }

        /// <summary>
        /// Ensures the command query invalidates all cahces it could affect and 
        /// then calls the next command query handler in the chain.
        /// </summary>
        /// <param name="commandQuery">The state of the command query.</param>
        /// <param name="token">
        /// A System.Threading.CancellationToken to observe while waiting for the task to complete.
        /// </param>
        /// <returns>An awaitable task.</returns>
        public async Task<IResult<TResult>> ExecuteAsync(TCommandQuery commandQuery, CancellationToken token = default)
        {
            IResult<TResult> result = await _commandQueryHandler.ExecuteAsync(commandQuery, token).ConfigureAwait(false);

            if (result == null || !result.IsSuccessful)
            {
                return result;
            }

            await Task.WhenAll(_cacheInvalidators.AsParallel()
                    .Select(a => a.InvalidateCacheAsync(commandQuery, token)))
                .ConfigureAwait(false);

            return result;
        }
    }
}