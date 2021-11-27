using System;

namespace MetalCore.CQS.Exceptions
{
    /// <summary>
    /// This exception is thrown when a user does not have access to perform a specific action.
    /// </summary>
    public class NoPermissionException : Exception
    {
        /// <summary>
        /// Constructor that takes a message and inner exception.
        /// </summary>
        /// <param name="message">The message of the exception.</param>
        /// <param name="innerException">The inner exception.</param>
        public NoPermissionException(string message = null, Exception innerException = null)
            : base(message, innerException)
        {
        }
    }
}