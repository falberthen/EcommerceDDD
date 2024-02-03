namespace EcommerceDDD.CustomerManagement.Tests.Domain;

public class CustomerTests
{
    [Fact]
    public void CreatingCustomer_WithCustomerData_ShouldCreateCustomer()
    {
        // Given
        string email = "email@test.com";
        string name = "UserTest";
        string streetAddress = "Rue XYZ";
        decimal creditLimit = 1000;

        var customerData = new CustomerData(email, name, streetAddress, creditLimit);

        // When
        var customer = Customer.Create(customerData);

        // Then
        Assert.NotNull(customer);
        customer.Id.Value.Should().NotBe(Guid.Empty);
        customer.Email.Should().Be(email);
        customer.Name.Should().Be(name);
        customer.ShippingAddress.Should().Be(Address.FromStreetAddress(streetAddress));
        customer.CreditLimit.Should().Be(CreditLimit.Create(creditLimit));
    }

    [Fact]
    public void UpdateInformation_WithChangingCustomerData_ShouldUpdateCustomerInformation()
    {
        // Given
        string email = "email@test.com";
        string name = "UserTest";
        string shippingAddress = "Rue XYZ";
        decimal creditLimit = 1000;

        var customerData = new CustomerData(email, name, shippingAddress, creditLimit);
        var customer = Customer.Create(customerData);

        var newName = "UserTestUpdated";
        var newShippingAddress = "Rue X";
        var newCreditLimit = 2000;
        customerData = customerData with 
        { 
            Name = newName,
            ShippingAddress = newShippingAddress,
            CreditLimit = newCreditLimit
        };

        // When
        customer.UpdateInformation(customerData);

        // Then
        Assert.NotNull(customer);
        customer.Id.Value.Should().NotBe(Guid.Empty);
        customer.Email.Should().Be(email);
        customer.Name.Should().Be(newName);
        customer.ShippingAddress.Should().Be(Address.FromStreetAddress(newShippingAddress));
        customer.CreditLimit.Should().Be(CreditLimit.Create(newCreditLimit));
    }
}