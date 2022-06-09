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
        public async Task<IActionResult> GetCars([FromQuery] ListOfCarsQuery query, CancellationToken token = default) =>
            await _responseMediator.ExecuteAsync(query, token);

        [HttpPost(Name = "AddCar")]
        public async Task<IActionResult> AddCar([FromBody] AddCarCommand command, CancellationToken token = default) =>
            await _responseMediator.ExecuteAsync(command, token);

        [HttpPost(Name = "AddNewRandomCar")]
        public async Task<IActionResult> AddNewRandomCar([FromBody] AddNewRandomCarCommand command, CancellationToken token = default) =>
            await _responseMediator.ExecuteAsync(command, token);

        [HttpPut(Name = "UpdateLastCarModel")]
        public async Task<IActionResult> UpdateLastCarModel([FromBody] UpdateLastCarModelCommandQuery commandQuery, CancellationToken token = default) =>
            await _responseMediator.ExecuteAsync(commandQuery, token);

        [HttpDelete(Name = "DeleteLastCar")]
        public async Task<IActionResult> DeleteLastCar([FromBody] DeleteLastCarCommand command, CancellationToken token = default) =>
            await _responseMediator.ExecuteAsync(command, token);
    }
}