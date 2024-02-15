using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Security.Encryption
{
    public class EncryptionFaildException : Exception
    {
        #region Properties

        /// <summary>
        /// The name of the parameter that caused the exception.
        /// </summary>
        public string ParamName { get; private set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the System.Exception class.
        /// </summary>
        public EncryptionFaildException() : base() { }

        /// <summary>
        /// Initializes a new instance of the Tameenk.Core.Exceptions.EncryptionFaildException class with a specified
        /// error message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>   
        public EncryptionFaildException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the Tameenk.Core.Exceptions.EncryptionFaildException class with a specified
        /// error message and the exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for this exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.
        /// </param>
        public EncryptionFaildException(string message, Exception innerException) : base(message, innerException) { }

        /// <summary>
        /// Initializes an instance of the Tameenk.Core.Exceptions.EncryptionFaildException class with a specified
        /// error message and the name of the parameter that causes this exception.
        /// </summary>
        /// <param name="paramName">The name of the parameter that caused the exception.</param>
        /// <param name="message">A message that describes the error.</param>
        public EncryptionFaildException(string paramName, string message) : base(message)
        {
            ParamName = paramName;
        }

        public override string ToString()
        {
            return $"The entity not found based on this key: {ParamName} - {base.ToString()}";
        }

        /// <summary>
        /// Override the message to add the parameter name to the message
        /// </summary>
        public override string Message
        {
            get
            {
                return $"{base.Message}{Environment.NewLine}Parameter name: {ParamName}";
            }
        }
        
    }
}
