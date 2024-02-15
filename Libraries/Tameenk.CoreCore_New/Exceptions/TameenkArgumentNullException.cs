using System;

namespace Tameenk.Core.Exceptions
{
    public class TameenkArgumentNullException : ArgumentNullException
    {
        /// <summary>
        /// Initializes a new instance of the System.ArgumentNullException class.
        /// </summary>
        public TameenkArgumentNullException() :base(){ }

        /// <summary>
        /// Initializes a new instance of the Tameenk.Core.Exceptions.TameenkArgumentNullException class with the
        /// name of the parameter that causes this exception.
        /// </summary>
        /// <param name="paramName">The name of the parameter that caused the exception.</param>   
        public TameenkArgumentNullException(string paramName) : base(paramName) { }

        /// <summary>
        /// Initializes a new instance of the Tameenk.Core.Exceptions.TameenkArgumentNullException class with a specified
        /// error message and the exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for this exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.
        /// </param>
        public TameenkArgumentNullException(string message, Exception innerException) : base(message, innerException) { }

        /// <summary>
        /// Initializes an instance of the Tameenk.Core.Exceptions.TameenkArgumentNullException class with a specified
        /// error message and the name of the parameter that causes this exception.
        /// </summary>
        /// <param name="paramName">The name of the parameter that caused the exception.</param>
        /// <param name="message">A message that describes the error.</param>
        public TameenkArgumentNullException(string paramName, string message) : base(paramName, message) { }
    }
}
