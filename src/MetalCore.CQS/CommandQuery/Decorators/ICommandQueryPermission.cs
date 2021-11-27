using System.Threading;
using System.Threading.Tasks;

namespace MetalCore.CQS.CommandQuery
{
    /// <summary>
    /// This interface defines the execution of a permission check for a given command query.
    /// </summary>
    /// <typeparam name="TCommandQuery">The type of command query being handled.</typeparam>
    /// <typeparam name="TResult">The type of result being returned.</typeparam>
    public interface ICommandQueryPermission<in TCommandQuery, TResult> where TCommandQuery : ICommandQuery<TResult>
    {
        /// <summary>
        /// Verifies that the current user has permission to process this command query.
        /// </summary>
        /// <param name="commandQuery">The command query to validate.</param>
        /// <param name="token">
        /// A System.Threading.CancellationToken to observe while waiting for the task to complete.
        /// </param>
        /// <returns>An awaitable task.</returns>
        Task<bool> HasPermissionAsync(TCommandQuery commandQuery, CancellationToken token = default);
    }
}