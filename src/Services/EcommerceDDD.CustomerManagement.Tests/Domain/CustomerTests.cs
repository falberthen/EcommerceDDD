namespace EcommerceDDD.CustomerManagement.Tests.Domain;

public class CustomerTests
{
    [Fact]
    public void CreatingCustomer_WithCustomerData_ShouldCreateCustomer()
    {
        // Given
        string email = "email@test.com";
        string name = "UserTest";
        string shippingAddress = "Rue XYZ";
        decimal creditLimit = 1000;

		var customerData = new CustomerData(
			email, name, shippingAddress, creditLimit
		);

		// When
		var customer = Customer.Create(customerData);

        // Then
        Assert.NotNull(customer);
        Assert.NotEqual(customer.Id.Value, Guid.Empty);
        Assert.Equal(customer.Email, email);
		Assert.Equal(customer.Name, name);
		Assert.Equal(customer.ShippingAddress, Address.FromStreetAddress(shippingAddress));
		Assert.Equal(customer.CreditLimit, CreditLimit.Create(creditLimit));
    }

    [Fact]
    public void UpdateInformation_WithChangingCustomerData_ShouldUpdateCustomerInformation()
    {
        // Given
        string email = "email@test.com";
        string name = "UserTest";
        string shippingAddress = "Rue XYZ";
        decimal creditLimit = 1000;

		var customerData = new CustomerData(
			email, name, shippingAddress, creditLimit
		);
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
		Assert.NotEqual(customer.Id.Value, Guid.Empty);
		Assert.Equal(customer.Email, email);
		Assert.Equal(customer.Name, newName);
		Assert.Equal(customer.ShippingAddress, Address.FromStreetAddress(newShippingAddress));
		Assert.Equal(customer.CreditLimit, CreditLimit.Create(newCreditLimit));
	}
}