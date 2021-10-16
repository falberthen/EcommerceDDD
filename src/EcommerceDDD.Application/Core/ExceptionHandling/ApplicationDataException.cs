using System;

namespace EcommerceDDD.Application.Core.ExceptionHandling
{
    public class ApplicationDataException : Exception
    {
        public ApplicationDataException(string message) : base(message) { }
    }
}
