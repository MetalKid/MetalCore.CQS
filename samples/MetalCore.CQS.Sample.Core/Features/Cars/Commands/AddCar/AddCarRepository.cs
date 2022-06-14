using MetalCore.CQS.Mediators;
using MetalCore.CQS.Repository;
using MetalCore.CQS.Sample.Core.DataStore;
using System.Threading;
using System.Threading.Tasks;

namespace MetalCore.CQS.Sample.Core.Features.Cars.Commands.AddCar
{
    public class AddCarRepository : IRepository<AddCarCommand, CarEntity>
    {
        private readonly ICarDataStore _dataStore;
        private readonly IMapperMediator _mapperMediator;

        public AddCarRepository(ICarDataStore dataStore, IMapperMediator mapperMediator)
        {
            _dataStore = dataStore;
            _mapperMediator = mapperMediator;
        }

        public Task<CarEntity> ExecuteAsync(AddCarCommand request, CancellationToken token = default)
        {
            var entity = _mapperMediator.Map<CarEntity>(request);
            _dataStore.Cars.Add(entity);
            return Task.FromResult(entity);
        }
    }
}