namespace EcommerceDDD.Core.Exceptions.Types;

public class RecordNotFoundException(string message) : Exception(message) { }