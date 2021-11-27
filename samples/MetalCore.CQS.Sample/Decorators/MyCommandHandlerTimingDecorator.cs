using MetalCore.CQS.Command;
using MetalCore.CQS.Common;
using MetalCore.CQS.Common.Results;
using System;
using System.Threading.Tasks;

namespace MetalCore.CQS.Sample.Decorators
{
    public class MyCommandHandlerTimingDecorator<TCommand> : CommandHandlerTimingDecorator<TCommand>
        where TCommand : ICommand
    {
        /// <summary>
        /// Constructor to enable IoC decoration.
        /// </summary>
        /// <param name="CommandHandler">The next Command handler to call in the chain.</param>
        public MyCommandHandlerTimingDecorator(ICommandHandler<TCommand> CommandHandler) : base(CommandHandler)
        {
        }

        protected override Task HandleTimerEndAsync(TCommand Command, IResult result, long totalMilliseconds)
        {
            Console.WriteLine($"Call for '{typeof(TCommand).FullName}' took {totalMilliseconds}ms");
            return Task.CompletedTask;
        }

        protected override Task HandleTimerWarningAsync(TCommand Command, IResult result, long totalMilliseconds, ICqsTimingWarningThreshold warningThreshold)
        {
            Console.WriteLine($"Call for '{typeof(TCommand).FullName}' took {totalMilliseconds}ms, longer than the warning threshold of {warningThreshold.WarningThresholdMilliseconds}ms.");
            return Task.CompletedTask;
        }
    }
}
