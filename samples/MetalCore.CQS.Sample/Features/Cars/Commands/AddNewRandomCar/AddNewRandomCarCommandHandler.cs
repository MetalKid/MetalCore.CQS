using MetalCore.CQS.Command;
using MetalCore.CQS.Common.Results;
using MetalCore.CQS.Mediators;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MetalCore.CQS.Sample.Features.Cars.Commands.AddNewRandomCar
{
    public class AddNewRandomCarCommandHandler : ICommandHandler<AddNewRandomCarCommand>
    {
        private readonly IRepositoryMediator _repositoryMediator;

        public AddNewRandomCarCommandHandler(IRepositoryMediator repositoryMediator)
        {
            _repositoryMediator = repositoryMediator;
        }

        public async Task<IResult> ExecuteAsync(AddNewRandomCarCommand command, CancellationToken token = default)
        {
            AddNewRandomCarRequest request = GetRandomNewCar();
            await _repositoryMediator.ExecuteAsync(request, token);
            return ResultHelper.Successful();
        }

        private static AddNewRandomCarRequest GetRandomNewCar()
        {
            Random random = new Random();
            int value = random.Next(0, 2);
            if (value == 0)
            {
                return new AddNewRandomCarRequest { Make = "Chevy", Model = Guid.NewGuid().ToString(), Year = 2022 };
            }

            if (value == 1)
            {
                return new AddNewRandomCarRequest { Make = "Ford", Model = Guid.NewGuid().ToString(), Year = 2022 };
            }

            return new AddNewRandomCarRequest { Make = "Camry", Model = Guid.NewGuid().ToString(), Year = 2022 };
        }
    }
}