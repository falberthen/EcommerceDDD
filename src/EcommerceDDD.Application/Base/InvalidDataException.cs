using System;

namespace EcommerceDDD.Application.Base
{
    public class InvalidDataException : Exception
    {
        public InvalidDataException(string message) : base(message) { }
    }
}
