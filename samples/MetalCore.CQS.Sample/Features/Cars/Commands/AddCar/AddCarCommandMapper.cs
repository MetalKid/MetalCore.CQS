using MetalCore.CQS.Mapper;
using MetalCore.CQS.Sample.DataStore;


namespace MetalCore.CQS.Sample.Features.Cars.Commands.AddCar
{
    public class AddCarCommandMapperr : IMapper<AddCarCommand, CarEntity>
    {
        public CarEntity Map(AddCarCommand from, CarEntity to = null)
        {
            to ??= new CarEntity();

            to.Make = from.Make;
            to.Model = from.Model;
            to.Year = from.Year;

            return to;
        }
    }
}
