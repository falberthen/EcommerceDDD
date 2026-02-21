namespace EcommerceDDD.Core.Validation;

public class ValidationError : FluentResults.Error
{
	public ValidationError(string message) : base(message) { }
}
