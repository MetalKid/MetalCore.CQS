using MetalCore.CQS.CommandQuery;
using MetalCore.CQS.Common.Results;
using MetalCore.CQS.Exceptions;
using MetalCore.CQS.Sample.Core.DataStore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MetalCore.CQS.Sample.Core.Features.Cars.CommandQueries.UpdateLastCarModel
{
    public class UpdateLastCarModelCommandQueryHandler :
        ICommandQueryHandler<UpdateLastCarModelCommandQuery, UpdateLastCarModelDto>
    {
        private readonly ICarDataStore _dataStore;

        public UpdateLastCarModelCommandQueryHandler(ICarDataStore dataStore)
        {
            _dataStore = dataStore;
        }

        //NOTE: There is no cache invalidation added here, so it won't clear cache upon change.
        public async Task<IResult<UpdateLastCarModelDto>> ExecuteAsync(
            UpdateLastCarModelCommandQuery commandQuery, CancellationToken token = default)
        {
            string newModel = Guid.NewGuid().ToString();

            CarEntity car = _dataStore.Cars.LastOrDefault();
            if (car == null)
            {
                throw new DataNotFoundException("No cars left.");
            }

            car.Model = newModel;

            return await Task.FromResult(ResultHelper.Successful(new UpdateLastCarModelDto { NewModel = newModel }));
        }
    }
}
