using System;
using System.Runtime.Serialization;
using Factoriada.Services;

namespace Factoriada.Exceptions
{
    internal class AddressException : Exception
    {
        public AddressException(string message) : base(message)
        {
        }
    }
}