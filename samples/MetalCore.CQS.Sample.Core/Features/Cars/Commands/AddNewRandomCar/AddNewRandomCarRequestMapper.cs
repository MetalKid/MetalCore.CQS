using MetalCore.CQS.Mapper;
using MetalCore.CQS.Sample.Core.DataStore;

namespace MetalCore.CQS.Sample.Core.Features.Cars.Commands.AddNewRandomCar
{
    public class AddNewRandomCarRequestMapper : IMapper<AddNewRandomCarRequest, CarEntity>
    {
        public CarEntity Map(AddNewRandomCarRequest from, CarEntity to = null)
        {
            if (to == null)
            {
                to = new CarEntity();
            }

            to.Make = from.Make;
            to.Model = from.Model;
            to.Year = from.Year;

            return to;
        }
    }
}
