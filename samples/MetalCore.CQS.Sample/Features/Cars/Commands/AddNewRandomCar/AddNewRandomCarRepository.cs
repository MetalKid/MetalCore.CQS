﻿using MetalCore.CQS.Mediators;
using MetalCore.CQS.Repository;
using MetalCore.CQS.Sample.DataStore;
using System.Threading;
using System.Threading.Tasks;

namespace MetalCore.CQS.Sample.Features.Cars.Commands.AddNewRandomCar
{
    public class AddNewRandomCarRepository : IRepository<AddNewRandomCarRequest>
    {
        private readonly ICarDataStore _dataStore;
        private readonly IMapperMediator _mapperMediator;

        public AddNewRandomCarRepository(ICarDataStore dataStore, IMapperMediator mapperMediator)
        {
            _dataStore = dataStore;
            _mapperMediator = mapperMediator;
        }

        public Task ExecuteAsync(AddNewRandomCarRequest request, CancellationToken token = default)
        {
            CarEntity entity = _mapperMediator.Map<CarEntity>(request);
            _dataStore.Cars.Add(entity);
            return Task.CompletedTask;
        }
    }
}
