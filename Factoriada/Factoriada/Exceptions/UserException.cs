using System;
using System.Runtime.Serialization;
using Factoriada.Services;

namespace Factoriada.Exceptions
{
    [Serializable]
    internal class UserException : Exception
    {
        public UserException(string message) : base(message)
        {
        }
    }
}