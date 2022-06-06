using MetalCore.CQS.CommandQuery;
using MetalCore.CQS.Common;
using MetalCore.CQS.Common.Results;
using System;
using System.Threading.Tasks;

namespace MetalCore.CQS.Sample.Core.Decorators
{
    public class MyCommandQueryHandlerTimingDecorator<TCommandQuery, TResult> : CommandQueryHandlerTimingDecorator<TCommandQuery, TResult>
        where TCommandQuery : ICommandQuery<TResult>
    {
        /// <summary>
        /// Constructor to enable IoC decoration.
        /// </summary>
        /// <param name="commandQueryHandler">The next CommandQuery handler to call in the chain.</param>
        public MyCommandQueryHandlerTimingDecorator(ICommandQueryHandler<TCommandQuery, TResult> commandQueryHandler) : base(commandQueryHandler)
        {
        }

        protected override Task HandleTimerEndAsync(TCommandQuery CommandQuery, IResult result, long totalMilliseconds)
        {
            Console.WriteLine($"Call for '{typeof(TCommandQuery).FullName}' took {totalMilliseconds}ms");
            return Task.CompletedTask;
        }

        protected override Task HandleTimerWarningAsync(TCommandQuery CommandQuery, IResult result, long totalMilliseconds, ICqsTimingWarningThreshold warningThreshold)
        {
            Console.WriteLine($"Call for '{typeof(TCommandQuery).FullName}' took {totalMilliseconds}ms, longer than the warning threshold of {warningThreshold.WarningThresholdMilliseconds}ms.");
            return Task.CompletedTask;
        }
    }
}
