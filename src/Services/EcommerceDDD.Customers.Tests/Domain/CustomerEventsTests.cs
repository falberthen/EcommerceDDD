using Moq;
using EcommerceDDD.Customers.Domain.Events;

namespace EcommerceDDD.Customers.Tests.Domain;

public class CustomerEventsTests
{
    [Fact]
    public async Task CreatingCustomer_ReturnsCustomerRegisteredEvent()
    {
        // Given
        _checker.Setup(p => p.IsUserUnique(It.IsAny<string>()))
            .Returns(true);

        // When
        var customer = Customer.CreateNew(_email, _name, _address, _availableCreditLimit,
            _checker.Object);

        // Then
        var @event = customer.GetUncommittedEvents().LastOrDefault() as CustomerRegistered;
        Assert.NotNull(@event);
        @event.Should().BeOfType<CustomerRegistered>();
    }

    [Fact]
    public async Task UpdatingCustomer_ReturnsCustomerUpdatedEvent()
    {
        // Given
        _checker.Setup(p => p.IsUserUnique(It.IsAny<string>()))
            .Returns(true);

        const string newName = "A new name";
        const string newAddress = "A new address";
        var customer = Customer.CreateNew(_email, _name, _address, _availableCreditLimit,
            _checker.Object);

        // When
        customer.UpdateCustomerInfo(customer.Id, newName, newAddress, _availableCreditLimit);

        // Then
        var @event = customer.GetUncommittedEvents().LastOrDefault() as CustomerUpdated;
        Assert.NotNull(@event);
        @event.Should().BeOfType<CustomerUpdated>();
    }

    private const string _email = "email@test.com";
    private const string _name = "UserTest";
    private const string _address = "Rue XYZ";
    private const decimal _availableCreditLimit = 1000;
    private Mock<ICustomerUniquenessChecker> _checker = new();
}