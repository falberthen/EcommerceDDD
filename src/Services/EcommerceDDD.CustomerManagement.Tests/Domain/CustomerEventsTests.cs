namespace EcommerceDDD.CustomerManagement.Tests.Domain;

public class CustomerEventsTests
{
    [Fact]
    public void Create_WithCustomerData_ShouldApplyCustomerRegisteredEvent()
    {
        // Given
        var customerData = new CustomerData(_email, _name, _address, _creditLimit);

        // When
        var customer = Customer.Create(customerData);

        // Then
        var @event = customer.GetUncommittedEvents().LastOrDefault() as CustomerRegistered;
        Assert.NotNull(@event);
        @event.Should().BeOfType<CustomerRegistered>();
    }

    [Fact]
    public void UpdateInformation_WithCustomerData_ShouldApplyCustomerUpdatedEvent()
    {
        // Given
        var customerData = new CustomerData(_email, _name, _address, _creditLimit);
        var customer = Customer.Create(customerData);

        // When
        customer.UpdateInformation(customerData);

        // Then
        var @event = customer.GetUncommittedEvents().LastOrDefault() as CustomerUpdated;
        Assert.NotNull(@event);
        @event.Should().BeOfType<CustomerUpdated>();
    }

    private const string _email = "email@test.com";
    private const string _name = "UserTest";
    private const string _address = "Rue XYZ";
    private const decimal _creditLimit = 1000;
}