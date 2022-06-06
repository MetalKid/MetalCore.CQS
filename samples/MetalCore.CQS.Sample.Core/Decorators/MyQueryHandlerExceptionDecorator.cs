using MetalCore.CQS.Common.Results;
using MetalCore.CQS.Exceptions;
using MetalCore.CQS.Query;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MetalCore.CQS.Sample.Core.Decorators
{
    public class MyQueryHandlerExceptionDecorator<TQuery, TResult> : QueryHandlerExceptionDecorator<TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        public MyQueryHandlerExceptionDecorator(IQueryHandler<TQuery, TResult> queryHandler) : base(queryHandler)
        {
        }

        public override async Task<IResult<TResult>> ExecuteAsync(TQuery query,
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

        protected override Task HandleBrokenRuleExceptionAsync(TQuery query, BrokenRuleException ex)
        {
            Console.WriteLine($"Query '{typeof(TQuery).FullName}' threw a BrokenRuleException.");
            return Task.CompletedTask;
        }

        protected override Task HandleConcurrencyExceptionAsync(TQuery query, ConcurrencyException ex)
        {
            Console.WriteLine($"Query '{typeof(TQuery).FullName}' threw a ConcurrencyException.");
            return Task.CompletedTask;
        }

        protected override Task HandleDataNotFoundExceptionAsync(TQuery query, DataNotFoundException ex)
        {
            Console.WriteLine($"Query '{typeof(TQuery).FullName}' threw a DataNotFoundException.");
            return Task.CompletedTask;
        }

        protected override Task HandleNoPermissionExceptionAsync(TQuery query, NoPermissionException ex)
        {
            Console.WriteLine($"Query '{typeof(TQuery).FullName}' threw a NoPermissionException.");
            return Task.CompletedTask;
        }

        protected override Task HandleUserFriendlyExceptionAsync(TQuery query, UserFriendlyException ex)
        {
            Console.WriteLine($"Query '{typeof(TQuery).FullName}' threw a UserFriendlyException.");
            return Task.CompletedTask;
        }
    }
}