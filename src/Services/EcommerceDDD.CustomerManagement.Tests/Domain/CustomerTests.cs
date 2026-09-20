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
        decimal storeCredit = 1000;

		var customerData = new CustomerData(
			email, name, shippingAddress, storeCredit
		);

		// When
		var customer = Customer.Create(customerData);

        // Then
        Assert.NotNull(customer);
        Assert.NotEqual(customer.Id.Value, Guid.Empty);
        Assert.Equal(customer.Email, email);
		Assert.Equal(customer.Name, name);
		Assert.Equal(customer.ShippingAddress, Address.FromStreetAddress(shippingAddress));
		Assert.Equal(customer.StoreCredit, StoreCredit.Create(storeCredit));
    }

    [Fact]
    public void UpdateInformation_WithChangingCustomerData_ShouldUpdateCustomerInformation()
    {
        // Given
        string email = "email@test.com";
        string name = "UserTest";
        string shippingAddress = "Rue XYZ";
        decimal storeCredit = 1000;

		var customerData = new CustomerData(
			email, name, shippingAddress, storeCredit
		);
		var customer = Customer.Create(customerData);

        var newName = "UserTestUpdated";
        var newShippingAddress = "Rue X";
        var newStoreCredit = 2000;
        customerData = customerData with 
        { 
            Name = newName,
            ShippingAddress = newShippingAddress,
            StoreCredit = newStoreCredit
        };

        // When
        customer.UpdateInformation(customerData);

        // Then
        Assert.NotNull(customer);
		Assert.NotEqual(customer.Id.Value, Guid.Empty);
		Assert.Equal(customer.Email, email);
		Assert.Equal(customer.Name, newName);
		Assert.Equal(customer.ShippingAddress, Address.FromStreetAddress(newShippingAddress));
		Assert.Equal(customer.StoreCredit, StoreCredit.Create(newStoreCredit));
	}
}