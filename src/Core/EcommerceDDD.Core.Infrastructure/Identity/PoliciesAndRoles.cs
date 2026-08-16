namespace EcommerceDDD.Core.Infrastructure.Identity;

public class Policies
{    
    public const string CanRead = "CanRead";
    public const string CanWrite = "CanWrite";
    public const string CanDelete = "CanDelete";
}

public class Roles
{
    public const string Customer = "Customer";
	public const string M2MAccess = "M2MAccess";  // Machine to Machine
}

public class CustomClaimTypes
{
	/// <summary>
	/// Ties an authenticated user to the customer they own. Every ownership check reads this claim,
	/// so it must come from the token and never from client-supplied input.
	/// </summary>
	public const string CustomerId = "CustomerId";
}
