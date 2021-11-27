using MetalCore.CQS.Mediators;
using MetalCore.CQS.Repository;
using MetalCore.CQS.Sample.DataStore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MetalCore.CQS.Sample.Features.Cars.Queries.ListOfCars
{
    /// <summary>
    /// The Repository that comes with this is above the data source implementation. It is used as a way
    /// to break up the code from the Handler. Since it is in the same folder as the rest of the data,
    /// it is very easy to find everything related to this Query. You can also unit test these a
    /// lot easier.
    /// </summary>
    public class ListOfCarsRepository : IRepository<ListOfCarsQuery, ICollection<ListOfCarsDto>>
    {
        private readonly ICarDataStore _dataStore;
        private readonly IMapperMediator _mapperMediator;

        public ListOfCarsRepository(ICarDataStore dataStore, IMapperMediator mapperMediator)
        {
            _dataStore = dataStore;
            _mapperMediator = mapperMediator;
        }

        public Task<ICollection<ListOfCarsDto>> ExecuteAsync(ListOfCarsQuery request, CancellationToken token = default)
        {
            List<CarEntity> data = _dataStore.Cars.Where(a => string.IsNullOrEmpty(request.Make) || a.Make == request.Make).ToList();
            ICollection<ListOfCarsDto> result = data.Select(a => _mapperMediator.Map<ListOfCarsDto>(a)).ToList();
            return Task.FromResult(result);
        }
    }
}
