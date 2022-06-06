using MetalCore.CQS.Common;
using MetalCore.CQS.Common.Results;
using MetalCore.CQS.Query;
using System;
using System.Threading.Tasks;

namespace MetalCore.CQS.Sample.Core.Decorators
{
    public class MyQueryHandlerTimingDecorator<TQuery, TResult> : QueryHandlerTimingDecorator<TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        /// <summary>
        /// Constructor to enable IoC decoration.
        /// </summary>
        /// <param name="queryHandler">The next query handler to call in the chain.</param>
        public MyQueryHandlerTimingDecorator(IQueryHandler<TQuery, TResult> queryHandler) : base(queryHandler)
        {
        }

        protected override Task HandleTimerEndAsync(TQuery query, IResult result, long totalMilliseconds)
        {
            Console.WriteLine($"Call for '{typeof(TQuery).FullName}' took {totalMilliseconds}ms");
            return Task.CompletedTask;
        }

        protected override Task HandleTimerWarningAsync(TQuery query, IResult result, long totalMilliseconds, ICqsTimingWarningThreshold warningThreshold)
        {
            Console.WriteLine($"Call for '{typeof(TQuery).FullName}' took {totalMilliseconds}ms, longer than the warning threshold of {warningThreshold.WarningThresholdMilliseconds}ms.");
            return Task.CompletedTask;
        }
    }
}
