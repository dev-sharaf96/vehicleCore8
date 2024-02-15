using System;

namespace Tameenk.Integration.Dto.Najm
{
    public class NajmErrorException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the System.Exception class.
        /// </summary>
        public NajmErrorException() : base() { }

        /// <summary>
        /// Initializes a new instance of NajmErrorException class with a specified
        /// error message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>   
        public NajmErrorException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of NajmErrorException class with a specified
        /// error message and the exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for this exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// if no inner exception is specified.
        /// </param>
        public NajmErrorException(string message, Exception innerException) : base(message, innerException) { }

    }
}
