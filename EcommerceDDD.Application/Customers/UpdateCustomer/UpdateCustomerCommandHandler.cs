using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation.Results;
using EcommerceDDD.Application.Base.Commands;
using EcommerceDDD.Domain;
using EcommerceDDD.Domain.Customers.Events;

namespace EcommerceDDD.Application.Customers.UpdateCustomer
{
    public class UpdateCustomerCommandHandler : CommandHandler<UpdateCustomerCommand, CommandHandlerResult>
    {
        private readonly IEcommerceUnitOfWork _unitOfWork;

        public UpdateCustomerCommandHandler(
            IEcommerceUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public override async Task<Guid> RunCommand(UpdateCustomerCommand request, CancellationToken cancellationToken)
        {
            var customer = await _unitOfWork.CustomerRepository.GetCustomerById(request.CustomerId);
            if (customer != null)
            {
                customer.SetName(request.Name);
                customer.AddDomainEvent(new CustomerUpdatedEvent(customer.Id, customer.Name));
                _unitOfWork.CustomerRepository.UpdateCustomer(customer);
                await _unitOfWork.CommitAsync();
            }

            return customer.Id;
        }
    }
}
