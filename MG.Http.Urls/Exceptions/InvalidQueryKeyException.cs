using MG.Http.Urls.Resources;

namespace MG.Http.Urls.Exceptions
{
    /// <summary>
    /// An exception that is thrown when a key for a parameter is invalid, such as being null, empty, or 
    /// whitespace.
    /// </summary>
    /// <remarks>
    /// This exception is specific to scenarios where parameter keys must meet certain criteria and are 
    /// found invalid. It extends the <see cref="ArgumentException"/> to provide more specific context about
    /// the nature of the error.
    /// </remarks>
    public class InvalidQueryKeyException : ArgumentException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidQueryKeyException"/> class with a 
        /// system-supplied message that describes the error.
        /// </summary>
        /// <remarks>
        /// This constructor initializes the <see cref="Exception.Message"/> property of the new instance to
        /// a system-supplied message that describes the error, such as "The parameter key is invalid."
        /// This message is localized based on the current culture settings.
        /// </remarks>
        public InvalidQueryKeyException()
            : this(innerException: null)
        {
        }

        /// <inheritdoc cref="InvalidQueryKeyException(string, Exception)" path="/param"/>
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidQueryKeyException"/> class with a 
        /// specified error message.
        /// </summary>
        public InvalidQueryKeyException(string? message)
            : this(message, innerException: null)
        {
        }

        /// <inheritdoc cref="InvalidQueryKeyException(string, Exception)" path="/param"/>
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidQueryKeyException"/> class with a 
        /// specified error message and a reference to the inner exception that is the cause of this 
        /// exception.
        /// </summary>
        /// <remarks>
        /// An exception that is thrown as a direct result of a previous exception can include a reference 
        /// to the previous exception in the <see cref="Exception.InnerException"/> property. The inner 
        /// exception is a valuable debugging tool as it provides more detailed information about the cause 
        /// of the original error.
        /// </remarks>
        public InvalidQueryKeyException(Exception? innerException)
            : this(message: null, innerException)
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidQueryKeyException"/> class with a 
        /// specified error message and a reference to the inner exception that is the cause of this 
        /// exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a 
        /// <see langword="null"/> reference if no inner exception is specified.</param>
        public InvalidQueryKeyException(string? message, Exception? innerException)
            : base(message: ReturnMessageOrDefault(message), innerException)
        {
        }

        private static string ReturnMessageOrDefault(string? message)
        {
            return !string.IsNullOrWhiteSpace(message)
                ? message
                : Localizer.GetString(nameof(Messages.InvalidQueryKey_Message));
        }
    }
}

