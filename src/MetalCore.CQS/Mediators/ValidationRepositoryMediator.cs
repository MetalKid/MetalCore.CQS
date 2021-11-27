using MetalCore.CQS.Repository;
using MetalCore.CQS.Validation;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MetalCore.CQS.Mediators
{
    /// <summary>
    /// This class handles calling the correct validation repository automatically.
    /// </summary>
    public class ValidationRepositoryMediator : IValidationRepositoryMediator
    {
        private readonly Func<Type, dynamic> _getInstanceCallback;

        /// <summary>
        /// Constructor that takes an anonymous function to lookup an instance from the outside.
        /// </summary>
        /// <param name="getInstanceCallback">The callback to get an instance of a dynamic type.</param>
        public ValidationRepositoryMediator(Func<Type, dynamic> getInstanceCallback)
        {
            Guard.IsNotNull(getInstanceCallback, nameof(getInstanceCallback));

            _getInstanceCallback = getInstanceCallback;
        }

        /// <summary>
        /// Executes the validation repository for the given request automatically.
        /// </summary>
        /// <param name="request">The request to execute.</param>
        /// <param name="token">
        /// A System.Threading.CancellationToken to observe while waiting for the task to complete.
        /// </param>
        /// <returns>An awaitable task that returns the result of the request.</returns>
        public ICollection<Func<Task<BrokenRule>>> GetValidationRules<TRequest>(TRequest request,
            CancellationToken token = default)
        {
            Guard.IsNotNull(request, nameof(request));

            Type requestType = request.GetType();
            Type type = typeof(IValidationRepository<>).MakeGenericType(requestType);
            dynamic instance = _getInstanceCallback(type);
            if (instance == null)
            {
                throw new TypeLoadException(
                    $"No validation repository type found for request type: {requestType.FullName}");
            }

            dynamic specificRequest = Convert.ChangeType(request, requestType);
            return instance.GetValidationRules(specificRequest, token);
        }
    }
}