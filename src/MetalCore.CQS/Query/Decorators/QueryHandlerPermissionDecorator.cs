using MetalCore.CQS.Common.Results;
using MetalCore.CQS.Validation;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MetalCore.CQS.Query
{
    /// <summary>
    /// This class ensures the current user has permission to process this query.
    /// </summary>
    /// <typeparam name="TQuery">The type of query being handled.</typeparam>
    /// <typeparam name="TResult">The type of result being returned.</typeparam>
    public class QueryHandlerPermissionDecorator<TQuery, TResult> : IQueryHandler<TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        private readonly IQueryHandler<TQuery, TResult> _queryHandler;
        private readonly IEnumerable<IQueryPermission<TQuery, TResult>> _permissions;

        /// <summary>
        /// Constructor to enable IoC decoration.
        /// </summary>
        /// <param name="queryHandler">The next commqueryand handler to call in the chain.</param>
        /// <param name="permissions">The list of permission classes to execute for this type of query.</param>
        public QueryHandlerPermissionDecorator(
            IQueryHandler<TQuery, TResult> queryHandler,
            ICollection<IQueryPermission<TQuery, TResult>> permissions)
        {
            Guard.IsNotNull(queryHandler, nameof(queryHandler));
            Guard.IsNotNull(permissions, nameof(permissions));

            _queryHandler = queryHandler;
            _permissions = permissions;
        }

        /// <summary>
        /// Ensures current user has permission to process this query and 
        /// then calls the next query handler in the chain.
        /// </summary>
        /// <param name="query">The state of the query.</param>
        /// <param name="token">
        /// A System.Threading.CancellationToken to observe while waiting for the task to complete.
        /// </param>
        /// <returns>An awaitable task that returns the result of the query.</returns>
        public async Task<IResult<TResult>> ExecuteAsync(TQuery query, CancellationToken token = default)
        {
            bool[] result = await Task.WhenAll(_permissions.AsParallel().Select(a => a.HasPermissionAsync(query, token)))
                .ConfigureAwait(false);

            if (result.Any(hasPermission => !hasPermission))
            {
                return ResultHelper.NoPermissionError<TResult>();
            }

            return await _queryHandler.ExecuteAsync(query, token).ConfigureAwait(false);
        }
    }
}