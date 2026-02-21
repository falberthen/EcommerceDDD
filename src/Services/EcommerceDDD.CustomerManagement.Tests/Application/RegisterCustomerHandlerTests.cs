namespace EcommerceDDD.CustomerManagement.Tests.Application;

public class RegisterCustomerHandlerTests
{
	[Fact]
	public async Task Register_WithCommand_ShouldRegisterCustomer()
	{
		// Given
		Guid customerId = Guid.NewGuid();

		_checker.IsUniqueAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
			.Returns(true);

		var apiClient = new IdentityServerClient(_requestAdapter);
		var confirmation = _password;
		var registerCommand = RegisterCustomer
			.Create(_email, _password, confirmation, _name, _streetAddress, _creditLimit);
		var commandHandler = new RegisterCustomerHandler(
			apiClient, _checker, _dummyRepository);

		// When
		await commandHandler.HandleAsync(registerCommand, CancellationToken.None);

		// Then
		var addedCustomer = _dummyRepository.AggregateStream.First().Aggregate;
		Assert.Equal(addedCustomer.Email, registerCommand.Email);
		Assert.Equal(addedCustomer.Name, registerCommand.Name);
		Assert.Equal(addedCustomer.ShippingAddress, Address.FromStreetAddress(_streetAddress));
	}

	[Fact]
	public async Task Register_WithExistingEmail_ShouldReturnFailure()
	{
		// Given       
		_checker.IsUniqueAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
			.Returns(false);

		var apiClient = new IdentityServerClient(_requestAdapter);
		var confirmation = _password;
		var registerCommand = RegisterCustomer
			.Create(_email, _password, confirmation, _name, _streetAddress, _creditLimit);
		var commandHandler = new RegisterCustomerHandler(
			apiClient, _checker, _dummyRepository);

		// When
		var result = await commandHandler.HandleAsync(registerCommand, CancellationToken.None);

		// Then
		Assert.True(result.IsFailed);
	}

	public const string _email = "email@test.com";
	public const string _name = "UserTest";
	public const string _password = "p4ssw0rd";
	public const string _streetAddress = "Rue XYZ";
	public const decimal _creditLimit = 1000;
	private IEmailUniquenessChecker _checker = Substitute.For<IEmailUniquenessChecker>();
	private IRequestAdapter _requestAdapter = Substitute.For<IRequestAdapter>();
	private DummyEventStoreRepository<Customer> _dummyRepository = new DummyEventStoreRepository<Customer>();
}