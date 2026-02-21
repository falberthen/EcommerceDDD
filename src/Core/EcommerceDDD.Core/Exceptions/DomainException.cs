namespace EcommerceDDD.Core.Exceptions;

public class DomainException(string message, Exception? exception = null) : Exception(message, exception) { }