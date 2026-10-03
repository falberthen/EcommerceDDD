namespace EcommerceDDD.CustomerManagement.Application.RegisteringCustomer;

/// <summary>
/// Finds the customer registered under an e-mail, if any.
/// </summary>
public interface ICustomerEmailLookup
{
	Task<Guid?> FindCustomerIdAsync(string customerEmail, CancellationToken cancellationToken);
}
