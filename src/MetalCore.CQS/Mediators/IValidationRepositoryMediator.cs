using MetalCore.CQS.Validation;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MetalCore.CQS.Mediators
{
    /// <summary>
    /// This interface defines calling the correct validation repository automatically.
    /// </summary>
    public interface IValidationRepositoryMediator
    {
        /// <summary>
        /// Executes the query handler for the given query automatically.
        /// </summary>
        /// <param name="request">The request to validate.</param>
        /// <param name="token">
        /// A System.Threading.CancellationToken to observe while waiting for the task to complete.
        /// </param>
        /// <returns>An awaitable task that returns whether the request is valid.</returns>
        ICollection<Func<Task<BrokenRule>>> GetValidationRules<TRequest>(
            TRequest request,
            CancellationToken token = default);
    }
}