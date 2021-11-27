using System;

namespace MetalCore.CQS.Exceptions
{
    /// <summary>
    /// This exception is thrown when a an expected item is not found.
    /// </summary>
    public class DataNotFoundException : Exception
    {
        /// <summary>
        /// Constructor that takes a message and inner exception.
        /// </summary>
        /// <param name="message">The message of the exception.</param>
        /// <param name="innerException">The inner exception.</param>
        public DataNotFoundException(string message = null, Exception innerException = null)
            : base(message, innerException)
        {
        }
    }
}