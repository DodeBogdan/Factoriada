using System;

namespace Factoriada.Exceptions
{
    internal class InvalidBillException : Exception
    {
        public InvalidBillException(string message) : base(message)
        {
        }
    }
}