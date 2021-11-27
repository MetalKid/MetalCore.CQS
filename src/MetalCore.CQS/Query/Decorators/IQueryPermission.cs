using System.Threading;
using System.Threading.Tasks;

namespace MetalCore.CQS.Query
{
    /// <summary>
    /// This interface defines the execution of a permission check for a given query.
    /// </summary>
    /// <typeparam name="TQuery">The type of query being handled.</typeparam>
    /// <typeparam name="TResult">The type of result being returned.</typeparam>
    public interface IQueryPermission<in TQuery, TResult> where TQuery : IQuery<TResult>
    {
        /// <summary>
        /// Verifies that the current user has permission to process this query.
        /// </summary>
        /// <param name="query">The query to validate.</param>
        /// <param name="token">
        /// A System.Threading.CancellationToken to observe while waiting for the task to complete.
        /// </param>
        /// <returns>An awaitable task.</returns>
        Task<bool> HasPermissionAsync(TQuery query, CancellationToken token = default);
    }
}