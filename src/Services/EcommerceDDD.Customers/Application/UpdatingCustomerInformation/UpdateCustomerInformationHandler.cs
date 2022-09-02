using EcommerceDDD.Customers.Domain;
using EcommerceDDD.Core.CQRS.CommandHandling;
using EcommerceDDD.Core.Persistence;

namespace EcommerceDDD.Customers.Api.Application.UpdatingCustomerInformation;

public class UpdateCustomerInformationHandler : CommandHandler<UpdateCustomerInformation>
{
    private readonly IEventStoreRepository<Customer> _customerWriteRepository;

    public UpdateCustomerInformationHandler(
        IEventStoreRepository<Customer> customerWriteRepository)
    {
        _customerWriteRepository = customerWriteRepository;
    }

    public override async Task Handle(UpdateCustomerInformation command, CancellationToken cancellationToken)
    {
        var customer = await _customerWriteRepository
            .FetchStream(command.CustomerId.Value);

        if (customer == null)
            throw new ApplicationException("Customer not found.");

        customer.UpdateCustomerInfo(command.CustomerId, command.Name, command.Address);

        await _customerWriteRepository
            .AppendEventsAsync(customer, cancellationToken);
    }
}

public record UpdateUserRequest(string Email, string Password, string PasswordConfirm);