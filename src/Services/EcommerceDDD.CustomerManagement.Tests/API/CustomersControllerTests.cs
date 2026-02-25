namespace EcommerceDDD.CustomerManagement.Tests;

public class CustomersControllerTests
{
	public CustomersControllerTests()
	{
		_customersController = new CustomersController(_commandBus, _queryBus);
		_customersInternalController = new CustomersInternalController(_commandBus, _queryBus);
	}

	[Fact]
	public async Task ListHistory_WithCustomerId_ShouldReturnListOfCustomerEventHistory()
	{
		// Given
		var customerId = Guid.NewGuid();
		var expectedData = new List<CustomerEventHistory>
		{
			new CustomerEventHistory(
				Guid.NewGuid(),
				customerId,
				typeof(CustomerRegistered).Name,
				"event data",
				DateTime.UtcNow
			),
			new CustomerEventHistory(
				Guid.NewGuid(),
				customerId,
				typeof(CustomerUpdated).Name,
				"event data",
				DateTime.UtcNow
			)
		};

		_queryBus.SendAsync(Arg.Any<GetCustomerEventHistory>(), CancellationToken.None)
			.Returns(Result.Ok<IReadOnlyList<CustomerEventHistory>>(expectedData));

		// When
		var response = await _customersController.ListHistory(CancellationToken.None);

		// Then
		var okResult = Assert.IsType<OkObjectResult>(response);
		Assert.IsAssignableFrom<IReadOnlyList<CustomerEventHistory>>(okResult.Value);
	}

	[Fact]
	public async Task Register_RegisterCustomerRequest_ShouldRegisterCustomer()
	{
		// Given
		var request = new RegisterCustomerRequest()
		{
			Email = "customer@test.com",
			Name = "CustomerX",
			Password = "p4$$w0rd",
			PasswordConfirm = "p4$$w0rd",
			ShippingAddress = "Infinite loop street",
			CreditLimit = 1000
		};

		_commandBus.SendAsync(Arg.Any<RegisterCustomer>(), Arg.Any<CancellationToken>())
			.Returns(Result.Ok());

		// When
		var response = await _customersController
			.Register(request, CancellationToken.None);

		// Then
		Assert.IsType<OkResult>(response);
	}

	[Fact]
	public async Task UpdateInformation_RegisterCustomerRequest_ShouldUpdateInformation()
	{
		// Given
		var customerId = Guid.NewGuid();
		var request = new UpdateCustomerRequest
		{
			Name = "CustomerX",
			ShippingAddress = "Infinite loop street",
			CreditLimit = 1000m
		};

		_commandBus.SendAsync(Arg.Any<UpdateCustomerInformation>(), Arg.Any<CancellationToken>())
			.Returns(Result.Ok());

		// When
		var response = await _customersController
			.UpdateInformation(request, CancellationToken.None);

		// Then
		Assert.IsType<OkResult>(response);
	}

	#region INTERNAL
	[Fact]
	public async Task GetDetailsByCustomerId_WithCustomerId_ShouldReturnCustomerDetails()
	{
		// Given
		var customerId = Guid.NewGuid();
		var expectedData = new CustomerDetails
		{
			Id = customerId,
			Email = "customer@test.com",
			Name = "CustomerX",
			ShippingAddress = "Infinite loop street",
			CreditLimit = 1000
		};

		_queryBus.SendAsync(Arg.Any<GetCustomerDetailsById>(), CancellationToken.None)
			.Returns(Result.Ok<CustomerDetails>(expectedData));

		// When
		var response = await _customersInternalController
			.GetDetailsByCustomerId(customerId, CancellationToken.None);

		// Then
		var okResult = Assert.IsType<OkObjectResult>(response);
		Assert.IsAssignableFrom<CustomerDetails>(okResult.Value);
	}

	[Fact]
	public async Task GetCustomerCreditLimit_WithCustomerId_ShouldReturnCreditLimitModel()
	{
		// Given
		var customerId = Guid.NewGuid();
		var expectedData = new CreditLimitModel
		(
			customerId,
			10000
		);

		_queryBus.SendAsync(Arg.Any<GetCreditLimit>(), CancellationToken.None)
			.Returns(Result.Ok<CreditLimitModel>(expectedData));

		// When
		var response = await _customersInternalController
			.GetCustomerCreditLimit(customerId, CancellationToken.None);

		// Then
		var okResult = Assert.IsType<OkObjectResult>(response);
		Assert.IsAssignableFrom<CreditLimitModel>(okResult.Value);
	}

	#endregion

	private CustomersController _customersController;
	private CustomersInternalController _customersInternalController;
	private ICommandBus _commandBus = Substitute.For<ICommandBus>();
	private IQueryBus _queryBus = Substitute.For<IQueryBus>();
}
