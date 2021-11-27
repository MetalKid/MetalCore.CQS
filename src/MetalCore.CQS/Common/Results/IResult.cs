using MetalCore.CQS.Validation;
using System.Collections.Generic;

namespace MetalCore.CQS.Common.Results
{
    /// <summary>
    /// This interface defines the result of running code.
    /// </summary>
    public interface IResult
    {
        /// <summary>
        /// Gets whether the request was successful.
        /// </summary>
        bool IsSuccessful { get; }

        /// <summary>
        /// Gets any broken validation rules found.
        /// </summary>
        ICollection<BrokenRule> BrokenRules { get; }

        /// <summary>
        /// Gets whether there is an error due to the user not having permissions to perform the request.
        /// </summary>
        bool HasNoPermissionError { get; }

        /// <summary>
        /// Gets whether there was an error in finding the data of the request.
        /// </summary>
        bool HasDataNotFoundError { get; }

        /// <summary>
        /// Gets whether the target data items were updated previously.
        /// </summary>
        bool HasConcurrencyError { get; }

        /// <summary>
        /// Gets a specific error message to show to the user.
        /// </summary>
        string ErrorMessage { get; }
    }

    /// <summary>
    /// This interface defines the result of running code that returns data.
    /// </summary>
    /// <typeparam name="T">The type of data being returned.</typeparam>
    public interface IResult<out T> : IResult
    {
        /// <summary>
        /// Gets the resulting data from the request.
        /// </summary>
        T Data { get; }
    }
}