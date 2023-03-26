using MediatR;
using EcommerceDDD.Customers.Domain;
using EcommerceDDD.Core.Persistence;
using EcommerceDDD.Core.CQRS.CommandHandling;
using EcommerceDDD.Customers.Domain.Commands;
using EcommerceDDD.Core.Exceptions;

namespace EcommerceDDD.Customers.Api.Application.UpdatingCustomerInformation;

public class UpdateCustomerInformationHandler : ICommandHandler<UpdateCustomerInformation>
{
    private readonly IEventStoreRepository<Customer> _customerWriteRepository;

    public UpdateCustomerInformationHandler(
        IEventStoreRepository<Customer> customerWriteRepository)
    {
        _customerWriteRepository = customerWriteRepository;
    }

    public async Task Handle(UpdateCustomerInformation command, CancellationToken cancellationToken)
    {
        var customer = await _customerWriteRepository
            .FetchStreamAsync(command.CustomerId.Value);

        if (customer is null)
            throw new ArgumentNullException("Customer not found.");

        var customerData = new CustomerData(
            customer.Email,
            command.Name,
            command.ShippingAddress,
            command.CreditLimit);

        customer.UpdateCustomerInformation(customerData);

        await _customerWriteRepository
            .AppendEventsAsync(customer, cancellationToken);
    }
}

public record UpdateUserRequest(string Email, string Password, string PasswordConfirm);