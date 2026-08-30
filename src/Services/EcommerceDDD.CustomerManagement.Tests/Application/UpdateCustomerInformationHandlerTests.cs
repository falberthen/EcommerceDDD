namespace EcommerceDDD.CustomerManagement.Tests.Application;

public class UpdateCustomerInformationHandlerTests
{
	[Fact]
	public async Task UpdateCustomerInformation_WithCommand_ShouldUpdateCustomerInformation()
	{
		// Arrange
		string email = "email@test.com";
		string name = "UserTest";
		string streetAddress = "Rue XYZ";
		decimal creditLimit = 1000;

		var customerWriteRepository = new DummyEventStoreRepository<Customer>();
		var customerData = new CustomerData(email, name, streetAddress, creditLimit);
		var customer = Customer.Create(customerData);
		await customerWriteRepository.AppendEventsAndCommitAsync(customer);

		_userInfoRequester.GetCurrentUser()
			.Returns(new UserInfo()
			{
				UserId = Guid.NewGuid().ToString(),
				CustomerId = customer.Id.Value
			});

		var updateCommand = UpdateCustomerInformation
			.Create("New Name", "New Address", creditLimit);

		var commandHandler = new UpdateCustomerInformationHandler(
			_userInfoRequester, customerWriteRepository);

		// Act
		await commandHandler.HandleAsync(updateCommand, CancellationToken.None);
		var updatedCustomer = await customerWriteRepository
			.FetchForWritingAsync(customer.Id.Value);

		// Assert
		Assert.Equal("New Name", updatedCustomer.Name);
		Assert.Equal(updatedCustomer.ShippingAddress, Address.FromStreetAddress("New Address"));
	}

	private IUserInfoRequester _userInfoRequester = Substitute.For<IUserInfoRequester>();
}