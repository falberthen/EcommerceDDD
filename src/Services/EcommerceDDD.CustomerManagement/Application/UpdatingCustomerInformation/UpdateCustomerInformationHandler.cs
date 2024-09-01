namespace EcommerceDDD.CustomerManagement.Api.Application.UpdatingCustomerInformation;

public class UpdateCustomerInformationHandler(
    IEventStoreRepository<Customer> customerWriteRepository) : ICommandHandler<UpdateCustomerInformation>
{
    private readonly IEventStoreRepository<Customer> _customerWriteRepository = customerWriteRepository
		?? throw new ArgumentNullException(nameof(customerWriteRepository));

    public async Task Handle(UpdateCustomerInformation command, CancellationToken cancellationToken)
    {
        var customer = await _customerWriteRepository
            .FetchStreamAsync(command.CustomerId.Value)
            ?? throw new ArgumentNullException($"Customer {command.CustomerId.Value} not found.");

        var customerData = new CustomerData(
            customer.Email,
            command.Name,
            command.ShippingAddress,
            command.CreditLimit);

        customer.UpdateInformation(customerData);

        await _customerWriteRepository
            .AppendEventsAsync(customer, cancellationToken);
    }
}