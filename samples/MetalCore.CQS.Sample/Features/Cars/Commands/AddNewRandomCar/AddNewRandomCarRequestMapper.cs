using MetalCore.CQS.Mapper;
using MetalCore.CQS.Sample.DataStore;

namespace MetalCore.CQS.Sample.Features.Cars.Commands.AddNewRandomCar
{
    public class AddNewRandomCarRequestMapper : IMapper<AddNewRandomCarRequest, CarEntity>
    {
        public CarEntity Map(AddNewRandomCarRequest from, CarEntity to = null)
        {
            to ??= new CarEntity();

            to.Make = from.Make;
            to.Model = from.Model;
            to.Year = from.Year;

            return to;
        }
    }
}
