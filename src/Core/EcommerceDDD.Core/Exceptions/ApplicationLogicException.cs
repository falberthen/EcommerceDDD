namespace EcommerceDDD.Core.Exceptions;

public class ApplicationLogicException : Exception
{
    public ApplicationLogicException(string message) : base(message) { }
}
