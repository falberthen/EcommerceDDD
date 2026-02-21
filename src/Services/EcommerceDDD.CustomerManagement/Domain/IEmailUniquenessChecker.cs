namespace EcommerceDDD.CustomerManagement.Domain;

/// <summary>
/// Domain service for checking email uniqueness
/// </summary>
public interface IEmailUniquenessChecker
{
	Task<bool> IsUniqueAsync(string customerEmail, CancellationToken cancellationToken);
}