using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Core.Exceptions
{
    public class TameenkNullReferenceException : NullReferenceException
    {
        //
        // Summary:
        //     Initializes a new instance of the System.TameenkNullReferenceException class, setting
        //     the System.Exception.Message property of the new instance to a system-supplied
        //     message that describes the error, such as "The value 'null' was found where an
        //     instance of an object was required." This message takes into account the current
        //     system culture.
        public TameenkNullReferenceException() : base() { }
        //
        // Summary:
        //     Initializes a new instance of the System.TameenkNullReferenceException class with a
        //     specified error message.
        //
        // Parameters:
        //   message:
        //     A System.String that describes the error. The content of message is intended
        //     to be understood by humans. The caller of this constructor is required to ensure
        //     that this string has been localized for the current system culture.
        public TameenkNullReferenceException(string message) : base(message) { }
        //
        // Summary:
        //     Initializes a new instance of the System.TameenkNullReferenceException class with a
        //     specified error message and a reference to the inner exception that is the cause
        //     of this exception.
        //
        // Parameters:
        //   message:
        //     The error message that explains the reason for the exception.
        //
        //   innerException:
        //     The exception that is the cause of the current exception. If the innerException
        //     parameter is not null, the current exception is raised in a catch block that
        //     handles the inner exception.
        public TameenkNullReferenceException(string message, Exception innerException) : base(message, innerException) { }
        //
        // Summary:
        //     Initializes a new instance of the System.TameenkNullReferenceException class with serialized
        //     data.
        //
        // Parameters:
        //   info:
        //     The object that holds the serialized object data.
        //
        //   context:
        //     The contextual information about the source or destination.
        protected TameenkNullReferenceException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
