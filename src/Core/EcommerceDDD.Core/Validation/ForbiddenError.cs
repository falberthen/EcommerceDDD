namespace EcommerceDDD.Core.Validation;

public class ForbiddenError
	: FluentResults.Error
{
	public ForbiddenError(string message) : base(message) { }
}
