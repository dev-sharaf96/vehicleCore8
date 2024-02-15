﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Exceptions
{
    public class TameenkArgumentException : ArgumentException
    {
        /// <summary>
        /// Initializes a new instance of the TameenkArgumentException class.
        /// </summary>   
        public TameenkArgumentException() :base() { }

        /// <summary>
        /// Initializes a new instance of the TameenkArgumentException class with a specified
        /// error message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>   
        public TameenkArgumentException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the TameenkArgumentException class with a specified
        /// error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the innerException
        /// parameter is not a null reference, the current exception is raised in a catch
        /// block that handles the inner exception.</param>
        public TameenkArgumentException(string message, Exception innerException) : base(message, innerException) { }

        /// <summary>
        /// Initializes a new instance of the TameenkArgumentException class with a specified
        /// error message and the name of the parameter that causes this exception.
        /// </summary>
        /// <param name="message"> The error message that explains the reason for the exception.</param>
        /// <param name="paramName">The name of the parameter that caused the current exception.</param>   
        public TameenkArgumentException(string message, string paramName) : base(message, paramName) { }

        /// <summary>
        /// Initializes a new instance of the TameenkArgumentException class with a specified
        /// error message, the parameter name, and a reference to the inner exception that
        /// is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="paramName">The name of the parameter that caused the current exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception. 
        /// If the innerException parameter is not a null reference, the current exception is raised in a catch
        /// block that handles the inner exception.</param>
        public TameenkArgumentException(string message, string paramName, Exception innerException) : base(message, paramName, innerException) { }
    }
}