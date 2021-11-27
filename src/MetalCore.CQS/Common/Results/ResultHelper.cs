using MetalCore.CQS.Validation;
using System.Collections.Generic;

namespace MetalCore.CQS.Common.Results
{
    /// <summary>
    /// This class aids in creating an IResult or IResult of T.
    /// </summary>
    public static class ResultHelper
    {
        /// <summary>
        /// Returns an IResult marked as successful.
        /// </summary>
        /// <returns>An IResult marked as successful.</returns>
        public static IResult Successful()
        {
            return Result.Successful();
        }

        /// <summary>
        /// Returns an IResult marked as successful with data.
        /// </summary>
        /// <typeparam name="T">The type of data being returned.</typeparam>
        /// <param name="data">The successful data.</param>
        /// <returns>An IResult marked as successful with data.</returns>
        public static IResult<T> Successful<T>(T data)
        {
            return Result<T>.Successful(data);
        }

        /// <summary>
        /// Returns an IResult marked with broken rules.
        /// </summary>
        /// <param name="brokenRules">The broken validation rules.</param>
        /// <returns>An IResult marked with broken rules.</returns>
        public static IResult ValidationError(ICollection<BrokenRule> brokenRules)
        {
            return Result.ValidationError(brokenRules);
        }

        /// <summary>
        /// Returns an IResult marked with broken rules.
        /// </summary>
        /// <typeparam name="T">The type of data being returned.</typeparam>
        /// <param name="brokenRules">The broken validation rules.</param>
        /// <returns>An IResult marked with broken rules.</returns>
        public static IResult<T> ValidationError<T>(ICollection<BrokenRule> brokenRules)
        {
            return Result<T>.ValidationError(brokenRules);
        }

        /// <summary>
        /// Returns an IResult marked with a no data found error.
        /// </summary>
        /// <returns>An IResult marked with NoDataFound.</returns>
        public static IResult NoDataFoundError()
        {
            return Result.NoDataFoundError();
        }

        /// <summary>
        /// Returns an IResult marked with a no data found error.
        /// </summary>
        /// <typeparam name="T">The type of data being returned.</typeparam>
        /// <returns>An IResult marked with NoDataFound.</returns>
        public static IResult<T> NoDataFoundError<T>()
        {
            return Result<T>.NoDataFoundError();
        }

        /// <summary>
        /// Returns an IResult marked with a concurrency error.
        /// </summary>
        /// <returns>An IResult marked with NoDataFound.</returns>
        public static IResult ConcurrencyError()
        {
            return Result.ConcurrencyError();
        }

        /// <summary>
        /// Returns an IResult marked with a concurrency error.
        /// </summary>
        /// <typeparam name="T">The type of data being returned.</typeparam>
        /// <returns>An IResult marked with NoDataFound.</returns>
        public static IResult<T> ConcurrencyError<T>()
        {
            return Result<T>.ConcurrencyError();
        }

        /// <summary>
        /// Returns an IResult marked with a no permission error.
        /// </summary>
        /// <returns>An IResult marked with a no permission error.</returns>
        public static IResult NoPermissionError()
        {
            return Result.NoPermissionError();
        }

        /// <summary>
        /// Returns an IResult marked with a no permission error.
        /// </summary>
        /// <typeparam name="T">The type of data being returned.</typeparam>
        /// <returns>An IResult marked with a no permission error.</returns>
        public static IResult<T> NoPermissionError<T>()
        {
            return Result<T>.NoPermissionError();
        }

        /// <summary>
        /// Returns an IResult marked with an error message.
        /// </summary>
        /// <param name="message">The error message to show to the user.</param>
        /// <returns>an IResult marked with an error message.</returns>
        public static IResult Error(string message)
        {
            return Result.Error(message);
        }

        /// <summary>
        /// Returns an IResult marked with an error message.
        /// </summary>
        /// <typeparam name="T">The type of data being returned.</typeparam>
        /// <param name="message">The error message to show to the user.</param>
        /// <returns>an IResult marked with an error message.</returns>
        public static IResult<T> Error<T>(string message)
        {
            return Result<T>.Error(message);
        }
    }
}