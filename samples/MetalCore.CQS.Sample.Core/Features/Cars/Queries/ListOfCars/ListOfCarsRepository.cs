using MetalCore.CQS.Mediators;
using MetalCore.CQS.Repository;
using MetalCore.CQS.Sample.Core.DataStore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MetalCore.CQS.Sample.Core.Features.Cars.Queries.ListOfCars
{
    /// <summary>
    /// The Repository that comes with this is above the data source implementation. It is used as a way
    /// to break up the code from the Handler. Since it is in the same folder as the rest of the data,
    /// it is very easy to find everything related to this Query. You can also unit test these a
    /// lot easier.  You can send any type in (the request) and get any result out (response).
    /// If there is no direct business logic, you can just send in the Query/Command/CommandQuery directly
    /// here and process it - or create some sort of other request as a substitute.  Sometimes, you
    /// need to call multiple repositories to get all the data, so this allows for that.  Plus,
    /// you could re-use these repositories across other queries/commands/commandqueries.
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
