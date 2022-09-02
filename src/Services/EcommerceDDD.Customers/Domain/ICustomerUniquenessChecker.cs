namespace EcommerceDDD.Customers.Domain;

public interface ICustomerUniquenessChecker
{
    bool IsUserUnique(string customerEmail);
}