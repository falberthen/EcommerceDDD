namespace EcommerceDDD.Core.Infrastructure.Messaging;

/// <summary>
/// Carries the errors of a failed Result returned by a message handler.
/// </summary>
public class FailedResultException(IResultBase result)
	: Exception(string.Join("; ", result.Errors.Select(e => e.Message)))
{
	public IReadOnlyList<IError> Errors { get; } = result.Errors;
}
