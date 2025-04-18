namespace EcommerceDDD.CustomerManagement.Tests;

public class CustomersControllerTests
{
	public CustomersControllerTests()
	{
		_customersController = new CustomersController(_commandBus, _queryBus);
	}

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
			.Returns(expectedData);

		// When
		var response = await _customersController
			.GetDetailsByCustomerId(customerId, CancellationToken.None);

		// Then
		var okResult = Assert.IsType<OkObjectResult>(response);
		var apiResponse = Assert.IsType<ApiResponse<CustomerDetails>>(okResult.Value);
		Assert.IsAssignableFrom<CustomerDetails>(apiResponse.Data);
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
			.Returns(expectedData);

		// When
		var response = await _customersController
			.GetCustomerCreditLimit(customerId, CancellationToken.None);

		// Then		
		var okResult = Assert.IsType<OkObjectResult>(response);
		var apiResponse = Assert.IsType<ApiResponse<CreditLimitModel>>(okResult.Value);
		Assert.IsAssignableFrom<CreditLimitModel>(apiResponse.Data);
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
			.Returns(expectedData);

		// When
		var response = await _customersController.ListHistory(CancellationToken.None);

		// Then		
		var okResult = Assert.IsType<OkObjectResult>(response);
		var apiResponse = Assert.IsType<ApiResponse<IList<CustomerEventHistory>>>(okResult.Value);
		Assert.IsAssignableFrom<IList<CustomerEventHistory>>(apiResponse.Data);
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

		await _commandBus.SendAsync(Arg.Any<RegisterCustomer>(), Arg.Any<CancellationToken>());

		// When
		var response = await _customersController
			.Register(request, Arg.Any<CancellationToken>());

		// Then		
		Assert.IsType<OkObjectResult>(response);
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

		await _commandBus.SendAsync(Arg.Any<RegisterCustomer>(), Arg.Any<CancellationToken>());

		// When
		var response = await _customersController
			.UpdateInformation(request, Arg.Any<CancellationToken>());

		// Then
		Assert.IsType<OkObjectResult>(response);
	}

	private CustomersController _customersController;
	private ICommandBus _commandBus = Substitute.For<ICommandBus>();
	private IQueryBus _queryBus = Substitute.For<IQueryBus>();
}