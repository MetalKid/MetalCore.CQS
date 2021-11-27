using MetalCore.CQS.Command;

namespace MetalCore.CQS.Sample.Features.Cars.Commands.AddCar
{
    public class AddCarCommand : ICommand
    {
        public string Make { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
    }
}
