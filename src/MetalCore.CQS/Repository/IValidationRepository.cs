using MetalCore.CQS.Validation;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MetalCore.CQS.Repository
{
    /// <summary>
    /// This interface defines a repository used to run validation rules.
    /// </summary>
    /// <typeparam name="TRequest">The type of request to validate.</typeparam>
    public interface IValidationRepository<in TRequest>
    {
        /// <summary>
        /// Returns tasks used to validate the request.
        /// </summary>
        /// <param name="request">The request to validate</param>
        /// <param name="token">
        /// A System.Threading.CancellationToken to observe while waiting for the task to complete.
        /// </param>
        /// <returns>List of tasks to vaidate the request.</returns>
        ICollection<Func<Task<BrokenRule>>> GetValidationRules(
            TRequest request, CancellationToken token = default);
    }
}