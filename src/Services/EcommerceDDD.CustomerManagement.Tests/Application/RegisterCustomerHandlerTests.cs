namespace EcommerceDDD.CustomerManagement.Tests.Application;

public class RegisterCustomerHandlerTests
{
	public RegisterCustomerHandlerTests()
	{
		_options.Value
			.Returns(new TokenIssuerSettings() { Authority = "http://url" });

		_htpRequester.PostAsync<IntegrationHttpResponse>(Arg.Any<string>(), Arg.Any<object>())!
		   .Returns(Task.FromResult(new IntegrationHttpResponse() { Success = true }));
	}

	[Fact]
	public async Task Register_WithCommand_ShouldRegisterCustomer()
	{
		// Given
		Guid customerId = Guid.NewGuid();

		_checker.IsUnique(Arg.Any<string>())
			.Returns(true);
		_htpRequester.PostAsync<IntegrationHttpResponse>(Arg.Any<string>(),
			new RegisterUserRequest(customerId, _email, _password, _password))
			.Returns(new IntegrationHttpResponse()
			{
				Data = JsonConvert.SerializeObject(new { userId = Guid.NewGuid() }),
				Success = true
			});

		var confirmation = _password;
		var registerCommand = RegisterCustomer
			.Create(_email, _password, confirmation, _name, _streetAddress, _creditLimit);
		var commandHandler = new RegisterCustomerHandler(
			_htpRequester, _checker, _options, _dummyRepository);

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
		var confirmation = _password;
		var registerCommand = RegisterCustomer
			.Create(_email, _password, confirmation, _name, _streetAddress, _creditLimit);
		var commandHandler = new RegisterCustomerHandler(
			_htpRequester, _checker, _options, _dummyRepository);

		// When & Then
		await Assert.ThrowsAsync<BusinessRuleException>(() =>
			commandHandler.HandleAsync(registerCommand, CancellationToken.None));
	}

	public const string _email = "email@test.com";
	public const string _name = "UserTest";
	public const string _password = "p4ssw0rd";
	public const string _streetAddress = "Rue XYZ";
	public const decimal _creditLimit = 1000;
	private IHttpRequester _htpRequester = Substitute.For<IHttpRequester>();
	private IEmailUniquenessChecker _checker = Substitute.For<IEmailUniquenessChecker>();
	private IOptions<TokenIssuerSettings> _options = Substitute.For<IOptions<TokenIssuerSettings>>();
	private DummyEventStoreRepository<Customer> _dummyRepository = new DummyEventStoreRepository<Customer>();
}

public record RegisterUserRequest(
	Guid CustomerId,
	string Email,
	string Password,
	string PasswordConfirm);