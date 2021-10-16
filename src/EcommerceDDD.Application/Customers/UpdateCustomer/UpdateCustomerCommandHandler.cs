using System;
using System.Threading;
using System.Threading.Tasks;
using EcommerceDDD.Domain;
using EcommerceDDD.Domain.Customers;
using EcommerceDDD.Application.Core.CQRS.CommandHandling;
using EcommerceDDD.Application.Core.ExceptionHandling;

namespace EcommerceDDD.Application.Customers.UpdateCustomer
{
    public class UpdateCustomerCommandHandler : CommandHandler<UpdateCustomerCommand, Guid>
    {
        private readonly IEcommerceUnitOfWork _unitOfWork;

        public UpdateCustomerCommandHandler(IEcommerceUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public override async Task<Guid> ExecuteCommand(UpdateCustomerCommand request, 
            CancellationToken cancellationToken)
        {
            var customerId = CustomerId.Of(request.CustomerId);
            var customer = await _unitOfWork.Customers
                .GetById(customerId, cancellationToken);

            if (customer == null)
                throw new ApplicationDataException("Customer not found.");

            customer.SetName(request.Name);
            await _unitOfWork.CommitAsync();

            return customer.Id.Value;
        }
    }
}
