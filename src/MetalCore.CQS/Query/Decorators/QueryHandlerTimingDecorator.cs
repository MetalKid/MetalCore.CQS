using MetalCore.CQS.Common;
using MetalCore.CQS.Common.Results;
using MetalCore.CQS.Validation;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace MetalCore.CQS.Query
{
    /// <summary>
    /// This decorator class is used to keep track of how long queries take to run.
    /// </summary>
    /// <typeparam name="TQuery">The type of query to time.</typeparam>
    /// <typeparam name="TResult">The type of result being returned.</typeparam>
    public abstract class QueryHandlerTimingDecorator<TQuery, TResult> : IQueryHandler<TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        private readonly IQueryHandler<TQuery, TResult> _queryHandler;

        /// <summary>
        /// Constructor to enable IoC decoration.
        /// </summary>
        /// <param name="queryHandler">The next query handler to call in the chain.</param>
        public QueryHandlerTimingDecorator(IQueryHandler<TQuery, TResult> queryHandler)
        {
            Guard.IsNotNull(queryHandler, nameof(queryHandler));

            _queryHandler = queryHandler;
        }

        /// <summary>
        /// Logs how long the query takes to run.
        /// </summary>
        /// <param name="query">The state of the query.</param>
        /// <param name="token">
        /// A System.Threading.CancellationToken to observe while waiting for the task to complete.
        /// </param>
        /// <returns>An awaitable task with a query result.</returns>
        public async Task<IResult<TResult>> ExecuteAsync(TQuery query,
            CancellationToken token = default)
        {
            IResult<TResult> result = null;
            Stopwatch timer = Stopwatch.StartNew();
            try
            {
                result = await _queryHandler.ExecuteAsync(query, token).ConfigureAwait(false);
            }
            finally
            {
                timer.Stop();

                await HandleTimerEndAsync(query, result, timer.ElapsedMilliseconds).ConfigureAwait(false);

                if (query is ICqsTimingWarningThreshold warningThreshold &&
                    timer.ElapsedMilliseconds >= warningThreshold.WarningThresholdMilliseconds)
                {
                    await HandleTimerWarningAsync(query, result, timer.ElapsedMilliseconds, warningThreshold)
                        .ConfigureAwait(false);
                }
            }

            return result;
        }

        /// <summary>
        /// This method is called after the query has finishes running.
        /// </summary>
        /// <param name="query">The state of the query.</param>
        /// <param name="result">The result of the query running.</param>
        /// <param name="totalMilliseconds">The total number of milliseconds it took to run the commnad.</param>
        /// <returns>An awaitable task.</returns>
        protected abstract Task HandleTimerEndAsync(TQuery query, IResult result, long totalMilliseconds);

        /// <summary>
        /// This method is called after the query has finishes running and it took longer than it should have.
        /// </summary>
        /// <param name="query">The state of the query.</param>
        /// <param name="result">The result of the query running.</param>
        /// <param name="totalMilliseconds">The total number of milliseconds it took to run the commnad.</param>
        /// <param name="warningThreshold">The data about the warning.</param>
        /// <returns>An awaitable task.</returns>
        protected abstract Task HandleTimerWarningAsync(
            TQuery query,
            IResult result,
            long totalMilliseconds,
            ICqsTimingWarningThreshold warningThreshold);
    }
}