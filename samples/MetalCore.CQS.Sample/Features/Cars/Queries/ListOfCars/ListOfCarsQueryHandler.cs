using MetalCore.CQS.Common.Results;
using MetalCore.CQS.Mediators;
using MetalCore.CQS.Query;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MetalCore.CQS.Sample.Features.Cars.Queries.ListOfCars
{
    /// <summary>
    /// This is the class that handles the entire query.  You can do whatever you like in the ExecuteAsync
    /// class.  You don't have to follow what is done here. Just know that this is where the main logic starts.
    /// 
    /// You can inject shared logic that can be re-used by other handlers.  You shouldn't have one handler
    /// inject another handler because that will cause you future grief and break SOLID.
    /// </summary>
    public class ListOfCarsQueryHandler : IQueryHandler<ListOfCarsQuery, ICollection<ListOfCarsDto>>
    {
        private readonly IRepositoryMediator _repositoryMediator;

        public ListOfCarsQueryHandler(IRepositoryMediator repositoryMediator)
        {
            _repositoryMediator = repositoryMediator;
        }

        public async Task<IResult<ICollection<ListOfCarsDto>>> ExecuteAsync(
            ListOfCarsQuery query, CancellationToken token = default)
        {
            ICollection<ListOfCarsDto> result = await _repositoryMediator.ExecuteAsync<ICollection<ListOfCarsDto>>(query, token);
            return ResultHelper.Successful(result);
        }
    }
}