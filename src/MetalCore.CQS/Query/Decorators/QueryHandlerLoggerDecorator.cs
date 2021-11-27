using MetalCore.CQS.Common.Results;
using MetalCore.CQS.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MetalCore.CQS.Query
{
    /// <summary>
    /// This decorator allows custom logging at start, end, and errors while processing the given query.
    /// </summary>
    /// <typeparam name="TQuery">The type of query being run.</typeparam>
    /// <typeparam name="TResult">The type of result the query returns.</typeparam>
    public class QueryHandlerLoggerDecorator<TQuery, TResult> : IQueryHandler<TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        private readonly IQueryHandler<TQuery, TResult> _queryHandler;
        private readonly IEnumerable<IQueryLogger<TQuery, TResult>> _queryLoggers;

        /// <summary>
        /// Constructor to enable IoC decoration.
        /// </summary>
        /// <param name="queryHandler">The next query handler to call in the chain.</param>
        /// <param name="queryLoggers">Optional loggers to log specific information for a query.</param>
        public QueryHandlerLoggerDecorator(
            IQueryHandler<TQuery, TResult> queryHandler,
            ICollection<IQueryLogger<TQuery, TResult>> queryLoggers)
        {
            Guard.IsNotNull(queryHandler, nameof(queryHandler));

            _queryHandler = queryHandler;
            _queryLoggers = queryLoggers;
        }

        /// <summary>
        /// Calls start/end/error log methods while running a specific query.
        /// </summary>
        /// <param name="query">The state of the query.</param>
        /// <param name="token">
        /// A System.Threading.CancellationToken to observe while waiting for the task to complete.
        /// </param>
        /// <returns>An awaitable task that returns the result of the query.</returns>
        public async Task<IResult<TResult>> ExecuteAsync(TQuery query,
            CancellationToken token = default)
        {
            try
            {
                if (_queryLoggers != null)
                {
                    await Task.WhenAll(_queryLoggers.AsParallel()
                        .Select(a => a.LogStartAsync(query, token))).ConfigureAwait(false);
                }

                IResult<TResult> result = await _queryHandler.ExecuteAsync(query, token).ConfigureAwait(false);

                if (_queryLoggers != null)
                {
                    await Task.WhenAll(_queryLoggers.AsParallel()
                    .Select(a => a.LogEndAsync(query, result, token))).ConfigureAwait(false);
                }

                return result;
            }
            catch (Exception ex)
            {
                await Task.WhenAll(_queryLoggers.AsParallel()
                    .Select(a => a.LogErrorAsync(query, ex, token))).ConfigureAwait(false);

                throw;
            }
        }
    }
}