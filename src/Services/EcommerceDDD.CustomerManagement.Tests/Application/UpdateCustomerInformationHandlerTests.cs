using Marten;
using Marten.Linq;

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

		// mock for write-repository
		var customerWriteRepository = new DummyEventStoreRepository<Customer>();
		var customerData = new CustomerData(email, name, streetAddress, creditLimit);
		var customer = Customer.Create(customerData);
		await customerWriteRepository.AppendEventsAsync(customer);

		var customerDetails = new CustomerDetails
		{
			Id = customer.Id.Value
		};

		var customerDetailsList = new List<CustomerDetails> { customerDetails }.AsQueryable();

		// mock for query session
		var martenQueryable = Substitute.For<IMartenQueryable<CustomerDetails>>();
		martenQueryable.Provider.Returns(customerDetailsList.Provider);
		martenQueryable.Expression.Returns(customerDetailsList.Expression);
		martenQueryable.ElementType.Returns(customerDetailsList.ElementType);
		martenQueryable.GetEnumerator().Returns(customerDetailsList.GetEnumerator());
		_querySession.Query<CustomerDetails>().Returns(martenQueryable);

		// mock for user info requester
		_userInfoRequester.RequestUserInfoAsync()
			.Returns(Task.FromResult(new UserInfo()
			{
				UserId = Guid.NewGuid().ToString(),
				CustomerId = customer.Id.Value
			}));

		var updateCommand = UpdateCustomerInformation
			.Create("New Name", "New Address", creditLimit);

		var commandHandler = new UpdateCustomerInformationHandler(
			_userInfoRequester, _querySession, customerWriteRepository);

		// Act
		await commandHandler.Handle(updateCommand, CancellationToken.None);
		var updatedCustomer = await customerWriteRepository
			.FetchStreamAsync(customer.Id.Value);

		// Assert
		updatedCustomer.Name.Should().Be("New Name");
		updatedCustomer.ShippingAddress.Should().Be(Address.FromStreetAddress("New Address"));
	}

	private IUserInfoRequester _userInfoRequester = Substitute.For<IUserInfoRequester>();
	private IQuerySession _querySession = Substitute.For<IQuerySession>();
}