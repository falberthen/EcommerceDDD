using EcommerceDDD.Customers.Domain;
using EcommerceDDD.Customers.Api.Application.UpdatingCustomerInformation;

namespace EcommerceDDD.Customers.Tests.Application;

public class UpdateCustomerInformationHandlerTests
{
    [Fact]
    public async Task UpdateCustomerInformation_WithCommand_ShouldUpdateCustomerInformation()
    {
        // Given
        string email = "email@test.com";
        string name = "UserTest";
        string streetAddress = "Rue XYZ";
        decimal creditLimit = 1000;

        var customerWriteRepository = new DummyEventStoreRepository<Customer>();
        var customerData = new CustomerData(email, name, streetAddress, creditLimit);
        var customer = Customer.Create(customerData);
        await customerWriteRepository.AppendEventsAsync(customer);

        var newName = "New Name";
        var newStreetAddress = "New Address";
        var updateCommand = UpdateCustomerInformation
            .Create(customer.Id, newName, newStreetAddress, creditLimit);
        var commandHandler = new UpdateCustomerInformationHandler(customerWriteRepository);

        // When
        await commandHandler.Handle(updateCommand, CancellationToken.None);
        var updatedCustomer = await customerWriteRepository.FetchStreamAsync(customer.Id.Value);

        // Then
        updatedCustomer.Name.Should().Be(newName);
        updatedCustomer.ShippingAddress.Should().Be(Address.FromStreetAddress(newStreetAddress));
    }
}