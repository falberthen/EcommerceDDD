using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EcommerceDDD.Domain;
using EcommerceDDD.Domain.Customers.Events;
using Microsoft.Extensions.DependencyInjection;
using MediatR;

namespace EcommerceDDD.Application.Customers.RegisterCustomer
{
    public class CustomerRegisteredEventHandler : INotificationHandler<CustomerRegisteredEvent>
    {
        private IEcommerceUnitOfWork _unitOfWork;
        private readonly IServiceScopeFactory _scopeFactory;

        public CustomerRegisteredEventHandler(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public async Task Handle(CustomerRegisteredEvent createdEvent, CancellationToken cancellationToken)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                _unitOfWork = scope.ServiceProvider.GetRequiredService<IEcommerceUnitOfWork>();

                var customer = await _unitOfWork.CustomerRepository
                    .GetCustomerById(createdEvent.AggregateId, cancellationToken);

                if (customer != null)
                {
                    //TODO: Implement send welcome e-mail

                    customer.SetWelcomeEmailSent(true);
                    _unitOfWork.CustomerRepository.UpdateCustomer(customer);
                    await _unitOfWork.CommitAsync();
                }
            }
        }
    }
}
