using MetalCore.CQS.Command;
using MetalCore.CQS.Common.Results;
using MetalCore.CQS.Exceptions;
using MetalCore.CQS.Sample.DataStore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MetalCore.CQS.Sample.Features.Cars.Commands.DeleteLastCar
{
    public class DeleteLastCarCommandHandler : ICommandHandler<DeleteLastCarCommand>
    {
        private readonly ICarDataStore _dataStore;

        public DeleteLastCarCommandHandler(ICarDataStore dataStore)
        {
            _dataStore = dataStore;
        }

        public Task<IResult> ExecuteAsync(DeleteLastCarCommand command, CancellationToken token = default)
        {
            if (_dataStore.Cars.Count <= 3)
            {
                throw new DataNotFoundException("Cannot delete the first 3 cars.");
            }

            _dataStore.Cars.Remove(_dataStore.Cars.Last());

            return Task.FromResult(ResultHelper.Successful());
        }
    }
}
