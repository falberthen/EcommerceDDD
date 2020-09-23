using EcommerceDDD.Domain;
using EcommerceDDD.Domain.Customers;

namespace EcommerceDDD.Domain.Services
{
    public class CustomerUniquenessChecker : ICustomerUniquenessChecker
    {
        private readonly IEcommerceUnitOfWork _unitOfWork;

        public CustomerUniquenessChecker(IEcommerceUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public bool IsUserUnique(string customerEmail)
        {
            var customer = _unitOfWork.CustomerRepository.GetByEmail(customerEmail).Result;
            return customer == null;
        }
    }
}
