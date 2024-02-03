namespace EcommerceDDD.Core.Infrastructure.Identity;

public class Policies
{
    public const string M2MAccess = "M2MAccess";  // Machine to Machine
    public const string CanRead = "CanRead";
    public const string CanWrite = "CanWrite";
    public const string CanDelete = "CanDelete";
}

public class Roles
{
    public const string Customer = "Customer";
}
