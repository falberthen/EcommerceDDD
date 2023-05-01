using EcommerceDDD.Customers.Domain;

namespace EcommerceDDD.Customers.Tests.Domain;

public class CustomerCreationTests
{
    [Fact]
    public void CreatingCustomer_WithCustomerData_ReturnsCustomer()
    {
        // Given
        string email = "email@test.com";
        string name = "UserTest";
        string address = "Rue XYZ";
        decimal creditLimit = 1000;

        var customerData = new CustomerData(email, name, address, creditLimit);

        // When
        var customer = Customer.Create(customerData);

        // Then
        Assert.NotNull(customer);
        customer.Id.Value.Should().NotBe(Guid.Empty);
        customer.Email.Should().Be(email);
        customer.Name.Should().Be(name);
        customer.ShippingAddress.Should().Be(Address.Create(address));
        customer.CreditLimit.Should().Be(CreditLimit.Create(creditLimit));
    }
}