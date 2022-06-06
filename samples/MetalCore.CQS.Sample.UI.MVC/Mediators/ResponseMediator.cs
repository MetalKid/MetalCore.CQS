using MetalCore.CQS.Command;
using MetalCore.CQS.CommandQuery;
using MetalCore.CQS.Common.Results;
using MetalCore.CQS.Mediators;
using MetalCore.CQS.Query;
using MetalCore.CQS.Validation;
using Microsoft.AspNetCore.Mvc;

namespace MetalCore.CQS.Sample.UI.MVC.Mediators
{
    public class ResponseMediator : IResponseMediator
    {
        private readonly ICqsMediator _mediator;

        public ResponseMediator(ICqsMediator mediator) 
        {
            Guard.IsNotNull(mediator, nameof(mediator));
            _mediator = mediator;
        }

        public async Task<IActionResult> ExecuteAsync(
            ICommand command, CancellationToken token = default)
        {
            return HandleResult(await _mediator.ExecuteAsync(command, token).ConfigureAwait(false), () => new OkResult());
        }

        public async Task<IActionResult> ExecuteAsync<TResponse>(
            ICommandQuery<TResponse> commandQuery, CancellationToken token = default)
        {
            return HandleResult(await _mediator.ExecuteAsync(commandQuery, token).ConfigureAwait(false),
                a => new OkObjectResult(a));
        }

        public async Task<IActionResult> ExecuteAsync<TResponse>(
            IQuery<TResponse> query, CancellationToken token = default)
        {
            return HandleResult(await _mediator.ExecuteAsync(query, token).ConfigureAwait(false), a => new OkObjectResult(a));
        }

        public IActionResult HandleResult<T>(IResult<T> result, Func<T?, IActionResult> okCallback)
        {
            if (result.IsSuccessful)
            {
                return okCallback(result.Data);
            }
            return HandleResult(result, () => okCallback(default));
        }

        public IActionResult HandleResult(MetalCore.CQS.Common.Results.IResult result, Func<IActionResult> okCallback)
        {
            if (result.IsSuccessful)
            {
                return okCallback();
            }
            if (result.BrokenRules?.Any() == true)
            {
                return new BadRequestObjectResult(new { result.BrokenRules });
            }
            if (result.HasConcurrencyError)
            {
                return new BadRequestObjectResult(new { Message = "Someone else has already changed this record. Please refresh and try again." });
            }
            if (result.HasNoPermissionError)
            {
                return new UnauthorizedResult();
            }
            if (result.HasDataNotFoundError)
            {
                return new NotFoundResult();
            }
            if (!string.IsNullOrEmpty(result.ErrorMessage))
            {
                return new BadRequestObjectResult(new { Message = result.ErrorMessage });
            }
            return new BadRequestObjectResult(new { Message = "An unexpected error occurred." });
        }
    }
}