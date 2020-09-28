namespace EcommerceDDD.Domain.Services
{
    public interface ICustomerUniquenessChecker
    {
        bool IsUserUnique(string customerEmail);
    }
}
