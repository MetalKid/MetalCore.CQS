using MetalCore.CQS.Command;
using MetalCore.CQS.Common.Results;
using MetalCore.CQS.Mediators;
using System.Threading;
using System.Threading.Tasks;

namespace MetalCore.CQS.Sample.Features.Cars.Commands.AddCar
{
    public class AddCarCommandHandler : ICommandHandler<AddCarCommand>
    {
        private readonly IRepositoryMediator _repositoryMediator;

        public AddCarCommandHandler(IRepositoryMediator repositoryMediator)
        {
            _repositoryMediator = repositoryMediator;
        }

        public async Task<IResult> ExecuteAsync(AddCarCommand command, CancellationToken token = default)
        {
            await _repositoryMediator.ExecuteAsync(command, token);
            return ResultHelper.Successful();
        }
    }
}