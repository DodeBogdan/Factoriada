using System;
using System.Runtime.Serialization;

namespace Factoriada.Exceptions
{
    internal class AddressException : Exception
    {
        public AddressException()
        {
        }

        public AddressException(string message) : base(message)
        {
        }

        public AddressException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected AddressException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}