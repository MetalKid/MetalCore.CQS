using MetalCore.CQS.Mapper;
using MetalCore.CQS.Sample.Core.DataStore;

namespace MetalCore.CQS.Sample.Core.Features.Cars.Queries.ListOfCars
{
    /// <summary>
    /// The IMapper that comes with this is completely optional. It is used as a way
    /// to break up the code from the Handler. Since it is in the same folder as the rest of the data,
    /// it is very easy to find everything related to this Query. You can also unit test these a
    /// lot easier.  However, you could also have a global implementation that can directly call
    /// Automapper or something else instead if need be.  You could even get fancy and call specific
    /// implementations if they existed (due to complexity) and then fallback to use Automapper.
    /// </summary>
    public class ListOfCarsMapper : IMapper<CarEntity, ListOfCarsDto>
    {
        public ListOfCarsDto Map(CarEntity from, ListOfCarsDto to = null)
        {
            if (to == null)
            {
                to = new ListOfCarsDto();
            }

            to.Make = from.Make;
            to.Model = from.Model;

            return to;
        }
    }
}
