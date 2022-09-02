using Moq;
using EcommerceDDD.Core.Testing;
using EcommerceDDD.Customers.Api.Application.UpdatingCustomerInformation;

namespace EcommerceDDD.Customers.Tests.Application;

public class UpdateCustomerCommandHandlerTests
{
    [Fact]
    public async Task Update_WithCommand_ShouldUpdateCustomer()
    {
        // Given
        _checker.Setup(p => p.IsUserUnique(It.IsAny<string>()))
            .Returns(true);
        
        var customerWriteRepository = new DummyEventStoreRepository<Customer>();
        var customer = Customer.CreateNew(_email, _name, _address, _checker.Object);
        await customerWriteRepository.AppendEventsAsync(customer);

        var newName = "New Name";
        var newAddress = "New Address";
        var command = new UpdateCustomerInformation(customer.Id, newName, newAddress);
        var commandHandler = new UpdateCustomerInformationHandler(customerWriteRepository);

        // When
        await commandHandler.Handle(command, CancellationToken.None);
        var updatedCustomer = await customerWriteRepository.FetchStream(customer.Id.Value);

        // Then
        updatedCustomer.Name.Should().Be(newName);
        updatedCustomer.Address.Should().Be(newAddress);
    }

    private const string _email = "email@test.com";
    private const string _name = "UserTest";
    private const string _address = "Rue XYZ";
    private Mock<ICustomerUniquenessChecker> _checker = new();
}