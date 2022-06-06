using MetalCore.CQS.Sample.Core.Features.Cars.CommandQueries.UpdateLastCarModel;
using MetalCore.CQS.Sample.Core.Features.Cars.Commands.AddCar;
using MetalCore.CQS.Sample.Core.Features.Cars.Commands.AddNewRandomCar;
using MetalCore.CQS.Sample.Core.Features.Cars.Commands.DeleteLastCar;
using MetalCore.CQS.Sample.Core.Features.Cars.Queries.ListOfCars;
using MetalCore.CQS.Sample.UI.MVC.Mediators;
using Microsoft.AspNetCore.Mvc;

namespace MetalCore.CQS.Sample.UI.MVC.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class CarsController : ControllerBase
    {
        private readonly IResponseMediator _responseMediator;

        public CarsController(IResponseMediator responseMediator) =>
            _responseMediator = responseMediator;

        [HttpGet(Name = "GetCars")]
        public async Task<IActionResult> GetCars([FromQuery] ListOfCarsQuery query) =>
            await _responseMediator.ExecuteAsync(query);

        [HttpPost(Name = "AddCar")]
        public async Task<IActionResult> AddCar([FromBody] AddCarCommand command) =>
            await _responseMediator.ExecuteAsync(command);

        [HttpPost(Name = "AddNewRandomCar")]
        public async Task<IActionResult> AddNewRandomCar([FromBody] AddNewRandomCarCommand command) =>
            await _responseMediator.ExecuteAsync(command);

        [HttpPut(Name = "UpdateLastCarModel")]
        public async Task<IActionResult> UpdateLastCarModel([FromBody] UpdateLastCarModelCommandQuery commandQuery) =>
            await _responseMediator.ExecuteAsync(commandQuery);

        [HttpDelete(Name = "DeleteLastCar")]
        public async Task<IActionResult> DeleteLastCar([FromBody] DeleteLastCarCommand command) =>
            await _responseMediator.ExecuteAsync(command);
    }
}