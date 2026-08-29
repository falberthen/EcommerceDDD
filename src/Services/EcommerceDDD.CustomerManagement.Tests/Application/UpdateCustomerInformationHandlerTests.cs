using EcommerceDDD.Core.Infrastructure.Marten;
using System.Linq.Expressions;
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
		_querySession.QueryFirstOrDefaultAsync<CustomerDetails>(
			Arg.Any<Expression<Func<CustomerDetails, bool>>>(), Arg.Any<CancellationToken>())
			.Returns(customerDetails);

		// mock for user info requester
		_userInfoRequester.GetCurrentUser()
			.Returns(new UserInfo()
			{
				UserId = Guid.NewGuid().ToString(),
				CustomerId = customer.Id.Value
			});

		var updateCommand = UpdateCustomerInformation
			.Create("New Name", "New Address", creditLimit);

		var commandHandler = new UpdateCustomerInformationHandler(
			_userInfoRequester, _querySession, customerWriteRepository);

		// Act
		await commandHandler.HandleAsync(updateCommand, CancellationToken.None);
		var updatedCustomer = await customerWriteRepository
			.FetchStreamAsync(customer.Id.Value);

		// Assert
		Assert.Equal("New Name", updatedCustomer.Name);
		Assert.Equal(updatedCustomer.ShippingAddress, Address.FromStreetAddress("New Address"));		
	}

	private IUserInfoRequester _userInfoRequester = Substitute.For<IUserInfoRequester>();
	private IQuerySessionWrapper _querySession = Substitute.For<IQuerySessionWrapper>();
}