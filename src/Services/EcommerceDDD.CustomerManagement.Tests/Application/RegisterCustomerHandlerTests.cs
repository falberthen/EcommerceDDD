namespace EcommerceDDD.CustomerManagement.Tests.Application;

public class RegisterCustomerHandlerTests
{
	[Fact]
	public async Task Register_WithNewEmail_ShouldRegisterCustomerAndItsUserUnderTheSameId()
	{
		// Given
		IdentityReturns(UserRegistrationStatus.Registered);
		var commandHandler = CreateHandler();

		// When
		var result = await commandHandler.HandleAsync(_command, CancellationToken.None);

		// Then
		Assert.True(result.IsSuccess);
		var addedCustomer = _dummyRepository.AggregateStream.Single().Aggregate;
		Assert.Equal(_command.Email, addedCustomer.Email);
		Assert.Equal(_command.Name, addedCustomer.Name);
		Assert.Equal(Address.FromStreetAddress(_streetAddress), addedCustomer.ShippingAddress);
		await _identityService.Received(1).RegisterUserAsync(
			addedCustomer.Id.Value, _command.Email, _password, _password, Arg.Any<CancellationToken>());
	}

	[Fact]
	public void Register_ShouldNormalizeEmail()
	{
		var command = RegisterCustomer.Create(" EMail@Test.com ", _password, _password, _name, _streetAddress, _storeCredit);

		Assert.Equal(_email, command.Email);
	}

	[Fact]
	public async Task Register_WithUnfinishedRegistration_ShouldFinishItWithTheExistingCustomer()
	{
		// Given
		var unfinished = await GivenUnfinishedRegistration();
		IdentityReturns(UserRegistrationStatus.Registered);
		var commandHandler = CreateHandler();

		// When
		var result = await commandHandler.HandleAsync(_command, CancellationToken.None);

		// Then
		Assert.True(result.IsSuccess);
		Assert.Single(_dummyRepository.AggregateStream); // no second customer, no pointless update
		await _identityService.Received(1).RegisterUserAsync(
			unfinished.Id.Value, _command.Email, _password, _password, Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task Register_WithUnfinishedRegistrationAndNewDetails_ShouldUpdateTheCustomer()
	{
		// Given
		var unfinished = await GivenUnfinishedRegistration();
		IdentityReturns(UserRegistrationStatus.Registered);
		var command = RegisterCustomer.Create(_email, "Another password", "Another password", "New name", "New street", 500);
		var commandHandler = CreateHandler();

		// When
		var result = await commandHandler.HandleAsync(command, CancellationToken.None);

		// Then
		Assert.True(result.IsSuccess);
		Assert.Equal(2, _dummyRepository.AggregateStream.Count);
		Assert.Equal("New name", unfinished.Name);
		Assert.Equal(Address.FromStreetAddress("New street"), unfinished.ShippingAddress);
	}

	[Fact]
	public async Task Register_WhenIdentityReportsEmailTaken_ShouldReturnValidationFailure()
	{
		// Given
		await GivenUnfinishedRegistration();
		IdentityReturns(UserRegistrationStatus.EmailTaken);
		var commandHandler = CreateHandler();

		// When
		var result = await commandHandler.HandleAsync(_command, CancellationToken.None);

		// Then
		Assert.True(result.IsFailed);
		Assert.IsType<ValidationError>(result.Errors.Single());
	}

	[Fact]
	public async Task Register_WhenIdentityRejectsPassword_ShouldReturnItsReasonAndKeepCustomerForRetry()
	{
		// Given
		_identityService.RegisterUserAsync(default, default!, default!, default!, default)
			.ReturnsForAnyArgs(new UserRegistrationResult(UserRegistrationStatus.Rejected, "Password too weak."));
		var commandHandler = CreateHandler();

		// When
		var result = await commandHandler.HandleAsync(_command, CancellationToken.None);

		// Then
		Assert.True(result.IsFailed);
		Assert.Equal("Password too weak.", result.Errors.Single().Message);
		Assert.Single(_dummyRepository.AggregateStream);
	}

	[Fact]
	public async Task Register_WhenIdentityIsUnreachable_ShouldThrowAndKeepCustomerForRetry()
	{
		// Given
		_identityService.RegisterUserAsync(default, default!, default!, default!, default)
			.ThrowsAsyncForAnyArgs(new System.Net.Http.HttpRequestException("unreachable"));
		var commandHandler = CreateHandler();

		// When / Then
		await Assert.ThrowsAsync<System.Net.Http.HttpRequestException>(
			() => commandHandler.HandleAsync(_command, CancellationToken.None));
		Assert.Single(_dummyRepository.AggregateStream);
	}

	private async Task<Customer> GivenUnfinishedRegistration()
	{
		var customer = Customer.Create(new CustomerData(_email, _name, _streetAddress, _storeCredit));
		await _dummyRepository.AppendEventsAndCommitAsync(customer);
		customer.ClearUncommittedEvents();
		_lookup.FindCustomerIdAsync(_email, Arg.Any<CancellationToken>())
			.Returns(customer.Id.Value);
		return customer;
	}

	private void IdentityReturns(UserRegistrationStatus status) =>
		_identityService.RegisterUserAsync(default, default!, default!, default!, default)
			.ReturnsForAnyArgs(new UserRegistrationResult(status));

	private RegisterCustomerHandler CreateHandler() =>
		new(_identityService, _lookup, _dummyRepository);

	public const string _email = "email@test.com";
	public const string _name = "UserTest";
	public const string _password = "p4ssw0rd";
	public const string _streetAddress = "Rue XYZ";
	public const decimal _storeCredit = 1000;
	private readonly RegisterCustomer _command = RegisterCustomer
		.Create(_email, _password, _password, _name, _streetAddress, _storeCredit);
	private readonly IIdentityService _identityService = Substitute.For<IIdentityService>();
	private readonly ICustomerEmailLookup _lookup = Substitute.For<ICustomerEmailLookup>();
	private readonly DummyEventStoreRepository<Customer> _dummyRepository = new();
}
