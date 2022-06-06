using MetalCore.CQS.CommandQuery;
using MetalCore.CQS.Common.Results;
using MetalCore.CQS.Exceptions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MetalCore.CQS.Sample.Core.Decorators
{
    public class MyCommandQueryHandlerExceptionDecorator<TCommandQuery, TResult> :
        CommandQueryHandlerExceptionDecorator<TCommandQuery, TResult>
        where TCommandQuery : ICommandQuery<TResult>
    {
        public MyCommandQueryHandlerExceptionDecorator(ICommandQueryHandler<TCommandQuery, TResult> commandQueryHandler) :
            base(commandQueryHandler)
        {
        }

        public override async Task<IResult<TResult>> ExecuteAsync(TCommandQuery query,
           CancellationToken token = default)
        {
            try
            {
                return await base.ExecuteAsync(query, token);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unknown error: " + ex.Message);
                return ResultHelper.Error<TResult>("Unexpected error.");
            }
        }

        protected override Task HandleBrokenRuleExceptionAsync(TCommandQuery CommandQuery, BrokenRuleException ex)
        {
            Console.WriteLine($"CommandQuery '{typeof(TCommandQuery).FullName}' threw a BrokenRuleException.  Rules broken:");
            foreach (Validation.BrokenRule rule in ex.BrokenRules)
            {
                Console.WriteLine($"{rule.RuleMessage} {rule.Relation}");
            }
            return Task.CompletedTask;
        }

        protected override Task HandleConcurrencyExceptionAsync(TCommandQuery query, ConcurrencyException ex)
        {
            Console.WriteLine($"CommandQuery '{typeof(TCommandQuery).FullName}' threw a ConcurrencyException.");
            return Task.CompletedTask;
        }

        protected override Task HandleDataNotFoundExceptionAsync(TCommandQuery query, DataNotFoundException ex)
        {
            Console.WriteLine($"CommandQuery '{typeof(TCommandQuery).FullName}' threw a DataNotFoundException. {ex.Message}");
            return Task.CompletedTask;
        }

        protected override Task HandleNoPermissionExceptionAsync(TCommandQuery query, NoPermissionException ex)
        {
            Console.WriteLine($"CommandQuery '{typeof(TCommandQuery).FullName}' threw a NoPermissionException.");
            return Task.CompletedTask;
        }

        protected override Task HandleUserFriendlyExceptionAsync(TCommandQuery query, UserFriendlyException ex)
        {
            Console.WriteLine($"CommandQuery '{typeof(TCommandQuery).FullName}' threw a UserFriendlyException.");
            return Task.CompletedTask;
        }
    }
}
