namespace EcommerceDDD.CustomerManagement.Domain;

/// <summary>
/// Finds the customer registered under an e-mail, if any.
/// </summary>
public interface ICustomerEmailLookup
{
	Task<Guid?> FindCustomerIdAsync(string customerEmail, CancellationToken cancellationToken);
}
