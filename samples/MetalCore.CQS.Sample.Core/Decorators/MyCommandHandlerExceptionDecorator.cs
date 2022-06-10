using MetalCore.CQS.Command;
using MetalCore.CQS.Common.Results;
using MetalCore.CQS.Exceptions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MetalCore.CQS.Sample.Core.Decorators
{
    public class MyCommandHandlerExceptionDecorator<TCommand> : CommandHandlerExceptionDecorator<TCommand>
        where TCommand : ICommand
    {
        public MyCommandHandlerExceptionDecorator(ICommandHandler<TCommand> commandHandler) :
            base(commandHandler)
        {
        }

        public override async Task<IResult> ExecuteAsync(TCommand query,
           CancellationToken token = default)
        {
            try
            {
                return await base.ExecuteAsync(query, token);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unknown error: " + ex.Message);
                return ResultHelper.Error("Unexpected error.");
            }
        }

        protected override Task HandleBrokenRuleExceptionAsync(TCommand command, BrokenRuleException ex)
        {
            Console.WriteLine($"Command '{typeof(TCommand).FullName}' threw a BrokenRuleException.  Rules broken:");
            foreach (var rule in ex.BrokenRules)
            {
                Console.WriteLine($"{rule.RuleMessage} {rule.Relation}");
            }
            return Task.CompletedTask;
        }

        protected override Task HandleConcurrencyExceptionAsync(TCommand query, ConcurrencyException ex)
        {
            Console.WriteLine($"Command '{typeof(TCommand).FullName}' threw a ConcurrencyException.");
            return Task.CompletedTask;
        }

        protected override Task HandleDataNotFoundExceptionAsync(TCommand query, DataNotFoundException ex)
        {
            Console.WriteLine($"Command '{typeof(TCommand).FullName}' threw a DataNotFoundException. {ex.Message}");
            return Task.CompletedTask;
        }

        protected override Task HandleNoPermissionExceptionAsync(TCommand query, NoPermissionException ex)
        {
            Console.WriteLine($"Command '{typeof(TCommand).FullName}' threw a NoPermissionException.");
            return Task.CompletedTask;
        }

        protected override Task HandleUserFriendlyExceptionAsync(TCommand query, UserFriendlyException ex)
        {
            Console.WriteLine($"Command '{typeof(TCommand).FullName}' threw a UserFriendlyException.");
            return Task.CompletedTask;
        }
    }
}
