using EcommerceDDD.ServiceClients.ApiGateway.Models;

namespace EcommerceDDD.CustomerManagement.Tests.Application;

public class RegisterCustomerHandlerTests
{
	public RegisterCustomerHandlerTests()
	{
		_options.Value
			.Returns(new TokenIssuerSettings() { Authority = "http://url" });
	}

	[Fact]
	public async Task Register_WithCommand_ShouldRegisterCustomer()
	{
		// Given
		Guid customerId = Guid.NewGuid();

		_checker.IsUnique(Arg.Any<string>())
			.Returns(true);

		var userCreationResult = new UserRegisteredResult()
		{
			Succeeded = true,
			UserId = Guid.NewGuid().ToString()
		};

		var apiClient = new ApiGatewayClient(_requestAdapter);
		// mocked kiota request
		_requestAdapter.SendAsync(
			Arg.Any<RequestInformation>(),
			Arg.Any<ParsableFactory<UserRegisteredResult>>(),
			Arg.Any<Dictionary<string, ParsableFactory<IParsable>>>(),
			Arg.Any<CancellationToken>())
			.Returns(userCreationResult);

		var confirmation = _password;
		var registerCommand = RegisterCustomer
			.Create(_email, _password, confirmation, _name, _streetAddress, _creditLimit);
		var commandHandler = new RegisterCustomerHandler(
			apiClient, _checker, _options, _dummyRepository);

		// When
		await commandHandler.HandleAsync(registerCommand, CancellationToken.None);

		// Then
		var addedCustomer = _dummyRepository.AggregateStream.First().Aggregate;
		Assert.Equal(addedCustomer.Email, registerCommand.Email);
		Assert.Equal(addedCustomer.Name, registerCommand.Name);
		Assert.Equal(addedCustomer.ShippingAddress, Address.FromStreetAddress(_streetAddress));
	}

	[Fact]
	public async Task Register_WithExistingEmail_ShouldThrownException()
	{
		// Given       
		_checker.IsUnique(Arg.Any<string>())
			.Returns(false);

		var apiClient = new ApiGatewayClient(_requestAdapter);
		// mocked kiota request
		_ = _requestAdapter.SendAsync(
			Arg.Any<RequestInformation>(),
			Arg.Any<ParsableFactory<UserRegisteredResult>>(),
			Arg.Any<Dictionary<string, ParsableFactory<IParsable>>>(),
			Arg.Any<CancellationToken>());

		var confirmation = _password;
		var registerCommand = RegisterCustomer
			.Create(_email, _password, confirmation, _name, _streetAddress, _creditLimit);
		var commandHandler = new RegisterCustomerHandler(
			apiClient, _checker, _options, _dummyRepository);

		// When & Then
		await Assert.ThrowsAsync<BusinessRuleException>(() =>
			commandHandler.HandleAsync(registerCommand, CancellationToken.None));
	}

	public const string _email = "email@test.com";
	public const string _name = "UserTest";
	public const string _password = "p4ssw0rd";
	public const string _streetAddress = "Rue XYZ";
	public const decimal _creditLimit = 1000;
	private IEmailUniquenessChecker _checker = Substitute.For<IEmailUniquenessChecker>();
	private IRequestAdapter _requestAdapter = Substitute.For<IRequestAdapter>();
	private DummyEventStoreRepository<Customer> _dummyRepository = new DummyEventStoreRepository<Customer>();
	private IOptions<TokenIssuerSettings> _options = Substitute.For<IOptions<TokenIssuerSettings>>();
}