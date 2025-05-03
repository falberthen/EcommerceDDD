namespace EcommerceDDD.Core.Exceptions.Types;

public class ApplicationLogicException(string message, Exception? exception = null) : Exception(message, exception) { }