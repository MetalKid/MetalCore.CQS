using MetalCore.CQS.Validation;
using System.Collections.Generic;

namespace MetalCore.CQS.Common.Results
{
    /// <summary>
    /// This class defines the result of running code.
    /// </summary>
    public class Result : IResult
    {
        /// <summary>
        /// Returns an IResult marked as successful.
        /// </summary>
        /// <returns>An IResult marked as successful.</returns>
        internal static IResult Successful()
        {
            return new Result { IsSuccessful = true };
        }

        /// <summary>
        /// Returns an IResult marked with broken rules.
        /// </summary>
        /// <param name="brokenRules">The broken validation rules.</param>
        /// <returns>An IResult marked with broken rules.</returns>
        internal static IResult ValidationError(ICollection<BrokenRule> brokenRules)
        {
            return new Result { BrokenRules = brokenRules };
        }

        /// <summary>
        /// Returns an IResult marked with a no data found error.
        /// </summary>
        /// <returns>An IResult marked with NoDataFound.</returns>
        internal static IResult NoDataFoundError()
        {
            return new Result { HasDataNotFoundError = true };
        }

        /// <summary>
        /// Returns an IResult marked with a concurrency error.
        /// </summary>
        /// <returns>An IResult marked with NoDataFound.</returns>
        internal static IResult ConcurrencyError()
        {
            return new Result { HasConcurrencyError = true };
        }

        /// <summary>
        /// Returns an IResult marked with a no permission error.
        /// </summary>
        /// <returns>An IResult marked with a no permission error.</returns>
        internal static IResult NoPermissionError()
        {
            return new Result { HasNoPermissionError = true };
        }

        /// <summary>
        /// Returns an IResult marked with an error message.
        /// </summary>
        /// <param name="message">The error message to show to the user.</param>
        /// <returns>an IResult marked with an error message.</returns>
        internal static IResult Error(string message)
        {
            return new Result { ErrorMessage = message };
        }

        /// <summary>
        /// Gets whether the request was successful.
        /// </summary>
        public bool IsSuccessful { get; internal set; }

        /// <summary>
        /// Gets any broken validation rules found.
        /// </summary>
        public ICollection<BrokenRule> BrokenRules { get; internal set; }

        /// <summary>
        /// Gets whether there is an error due to the user not having permissions to perform the request.
        /// </summary>
        public bool HasNoPermissionError { get; internal set; }

        /// <summary>
        /// Gets whether there was an error in finding the data of the request.
        /// </summary>
        public bool HasDataNotFoundError { get; internal set; }

        /// <summary>
        /// Gets whether the target data items were updated previously.
        /// </summary>
        public bool HasConcurrencyError { get; internal set; }

        /// <summary>
        /// Gets a specific error message to show to the user.
        /// </summary>
        public string ErrorMessage { get; internal set; }

        /// <summary>
        /// Internal constructor prevents newing up direct result objects.
        /// </summary>
        internal Result()
        {
        }
    }

    /// <summary>
    /// This class defines the result of running code that returns data.
    /// </summary>
    public class Result<T> : IResult<T>
    {
        /// <summary>
        /// Returns an IResult marked as successful with data.
        /// </summary>
        /// <typeparam name="T">The type of data being returned.</typeparam>
        /// <param name="data">The successful data.</param>
        /// <returns>An IResult marked as successful with data.</returns>
        internal static IResult<T> Successful(T data)
        {
            return new Result<T> { IsSuccessful = true, Data = data };
        }

        /// <summary>
        /// Gets whether the request was successful.
        /// </summary>
        public bool IsSuccessful { get; internal set; }

        /// <summary>
        /// Gets any broken validation rules found.
        /// </summary>
        public ICollection<BrokenRule> BrokenRules { get; internal set; }

        /// <summary>
        /// Gets whether there is an error due to the user not having permissions to perform the request.
        /// </summary>
        public bool HasNoPermissionError { get; internal set; }

        /// <summary>
        /// Gets whether there was an error in finding the data of the request.
        /// </summary>
        public bool HasDataNotFoundError { get; internal set; }

        /// <summary>
        /// Gets whether the target data items were updated previously.
        /// </summary>
        public bool HasConcurrencyError { get; internal set; }

        /// <summary>
        /// Gets a specific error message to show to the user.
        /// </summary>
        public string ErrorMessage { get; internal set; }

        /// <summary>
        /// Gets the resulting data from the request.
        /// </summary>
        public T Data { get; private set; }

        /// <summary>
        /// Returns an IResult marked with broken rules.
        /// </summary>
        /// <param name="brokenRules">The broken validation rules.</param>
        /// <returns>An IResult marked with broken rules.</returns>
        internal static IResult<T> ValidationError(ICollection<BrokenRule> brokenRules)
        {
            return new Result<T> { BrokenRules = brokenRules };
        }

        /// <summary>
        /// Returns an IResult marked with a no data found error.
        /// </summary>
        /// <returns>An IResult marked with NoDataFound.</returns>
        internal static IResult<T> NoDataFoundError()
        {
            return new Result<T> { HasDataNotFoundError = true };
        }

        /// <summary>
        /// Returns an IResult marked with a concurrency error.
        /// </summary>
        /// <returns>An IResult marked with NoDataFound.</returns>
        internal static IResult<T> ConcurrencyError()
        {
            return new Result<T> { HasConcurrencyError = true };
        }

        /// <summary>
        /// Returns an IResult marked with a no permission error.
        /// </summary>
        /// <returns>An IResult marked with a no permission error.</returns>
        internal static IResult<T> NoPermissionError()
        {
            return new Result<T> { HasNoPermissionError = true };
        }

        /// <summary>
        /// Returns an IResult marked with an error message.
        /// </summary>
        /// <param name="message">The error message to show to the user.</param>
        /// <returns>an IResult marked with an error message.</returns>
        internal static IResult<T> Error(string message)
        {
            return new Result<T> { ErrorMessage = message };
        }

        /// <summary>
        /// Internal constructor prevents newing up direct result objects.
        /// </summary>
        internal Result()
        {
        }
    }
}