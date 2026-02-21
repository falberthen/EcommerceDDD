namespace EcommerceDDD.Core.Validation;

public class RecordNotFoundError
	: FluentResults.Error
{
	public RecordNotFoundError(string message) : base(message) { }
}
