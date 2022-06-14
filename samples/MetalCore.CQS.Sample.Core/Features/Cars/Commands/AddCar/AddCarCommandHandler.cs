using MetalCore.CQS.Command;
using MetalCore.CQS.Common.Results;
using MetalCore.CQS.Mediators;
using MetalCore.CQS.PubSub;
using MetalCore.CQS.Sample.Core.DataStore;
using MetalCore.CQS.Sample.Core.Features.Cars.Events;
using System.Threading;
using System.Threading.Tasks;

namespace MetalCore.CQS.Sample.Core.Features.Cars.Commands.AddCar
{
    public class AddCarCommandHandler : ICommandHandler<AddCarCommand>
    {
        private readonly IRepositoryMediator _repositoryMediator;
        private readonly IPublisher _publisher;

        public AddCarCommandHandler(IRepositoryMediator repositoryMediator, IPublisher publisher)
        {
            _repositoryMediator = repositoryMediator;
            _publisher = publisher;
        }

        public async Task<IResult> ExecuteAsync(AddCarCommand command, CancellationToken token = default)
        {
            var car = await _repositoryMediator.ExecuteAsync<CarEntity>(command, token);

            await _publisher.PublishAsync(new NewCarAddedEvent(car), token);

            return ResultHelper.Successful();
        }
    }
}