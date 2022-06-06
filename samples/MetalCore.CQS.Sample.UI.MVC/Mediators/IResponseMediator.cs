using MetalCore.CQS.Command;
using MetalCore.CQS.CommandQuery;
using MetalCore.CQS.Query;
using Microsoft.AspNetCore.Mvc;

namespace MetalCore.CQS.Sample.UI.MVC.Mediators
{
    public interface IResponseMediator
    {
        Task<IActionResult> ExecuteAsync(
            ICommand command, CancellationToken token = default);

        Task<IActionResult> ExecuteAsync<TResponse>(
           ICommandQuery<TResponse> query, CancellationToken token = default);

        Task<IActionResult> ExecuteAsync<TResponse>(
            IQuery<TResponse> query, CancellationToken token = default);
    }
}