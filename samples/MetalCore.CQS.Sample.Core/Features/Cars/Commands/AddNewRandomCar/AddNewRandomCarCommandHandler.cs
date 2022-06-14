using MetalCore.CQS.Command;
using MetalCore.CQS.Common.Results;
using MetalCore.CQS.Mediators;
using MetalCore.CQS.PubSub;
using MetalCore.CQS.Sample.Core.DataStore;
using MetalCore.CQS.Sample.Core.Features.Cars.Events;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MetalCore.CQS.Sample.Core.Features.Cars.Commands.AddNewRandomCar
{
    public class AddNewRandomCarCommandHandler : ICommandHandler<AddNewRandomCarCommand>
    {
        private readonly IRepositoryMediator _repositoryMediator;
        private readonly IPublisher _publisher;

        public AddNewRandomCarCommandHandler(IRepositoryMediator repositoryMediator, IPublisher publisher)
        {
            _repositoryMediator = repositoryMediator;
            _publisher = publisher;
        }

        public async Task<IResult> ExecuteAsync(AddNewRandomCarCommand command, CancellationToken token = default)
        {
            AddNewRandomCarRequest request = GetRandomNewCar();
            var car = await _repositoryMediator.ExecuteAsync<CarEntity>(request, token);

            _publisher.PublishAndForgetAsync(new NewCarAddedEvent(car), token);

            return ResultHelper.Successful();
        }

        private static AddNewRandomCarRequest GetRandomNewCar()
        {
            Random random = new Random();
            int value = random.Next(0, 3);
            if (value == 0)
            {
                return new AddNewRandomCarRequest { Make = "Chevy", Model = Guid.NewGuid().ToString(), Year = 2022 };
            }

            if (value == 1)
            {
                return new AddNewRandomCarRequest { Make = "Ford", Model = Guid.NewGuid().ToString(), Year = 2022 };
            }

            return new AddNewRandomCarRequest { Make = "Toyota", Model = Guid.NewGuid().ToString(), Year = 2022 };
        }
    }
}