using System.Threading.Tasks;

namespace EcommerceDDD.Domain.Customers;

public interface ICustomerUniquenessChecker
{
    Task<bool> IsUserUnique(string customerEmail);
}