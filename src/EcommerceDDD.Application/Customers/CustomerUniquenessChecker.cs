using EcommerceDDD.Domain;
using EcommerceDDD.Domain.Customers;
using System.Threading.Tasks;

namespace EcommerceDDD.Application.Customers.DomainServices;

public class CustomerUniquenessChecker : ICustomerUniquenessChecker
{
    private readonly IEcommerceUnitOfWork _unitOfWork;

    public CustomerUniquenessChecker(IEcommerceUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> IsUserUnique(string customerEmail)
    {
        var customer = await _unitOfWork.Customers
            .GetByEmail(customerEmail);

        return customer == null;
    }
}
