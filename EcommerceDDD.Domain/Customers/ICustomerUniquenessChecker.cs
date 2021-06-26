namespace EcommerceDDD.Domain.Customers
{
    public interface ICustomerUniquenessChecker
    {
        bool IsUserUnique(string customerEmail);
    }
}
