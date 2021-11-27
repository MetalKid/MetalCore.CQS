using CacheManager.Core;
using MetalCore.CQS.Query;
using MetalCore.CQS.Sample.Decorators;
using MetalCore.CQS.Sample.Features.Cars.Queries.ListOfCars;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MetalCore.CQS.Sample.Features.Cars.Commands.AddNewRandomCar
{
    public class AddNewRandomCarCacheInvalidation : CacheManagerCommandInvalidateCache<AddNewRandomCarCommand>
    {
        public AddNewRandomCarCacheInvalidation(
          IQueryCacheRegion queryCacheRegion,
          ICacheManager<object> cache) : base(queryCacheRegion, cache)
        {
        }

        protected override async Task<ICollection<Type>> GetTypeOfQueriesToInvalidateAsync(CancellationToken token = default)
        {
            return await Task.FromResult(new List<Type> { typeof(ListOfCarsQuery) });
        }

        public override async Task InvalidateCacheAsync(AddNewRandomCarCommand command, CancellationToken token = default)
        {
            await base.InvalidateCacheAsync(command, token);
            Console.WriteLine("AddNewRandomCarCommand cleared cache for ListOfCarsQuery.");
        }
    }
}