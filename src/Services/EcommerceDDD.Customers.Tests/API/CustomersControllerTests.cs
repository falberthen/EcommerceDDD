namespace EcommerceDDD.Customers.Tests;

public class CustomersControllerTests
{
    public CustomersControllerTests()
    {
        var fakeToken = "yJhbGciOiJSUzI1NiIsImtpZCI6IjAzOUI0NUE1OThCMzE3RTRBQzc0M";
        _tokenRequester
            .Setup(m => m.GetUserTokenFromHttpContextAsync())
            .ReturnsAsync(fakeToken);

        _customersController = new CustomersController(
            _tokenRequester.Object,
            _commandBus.Object,
            _queryBus.Object);
    }

    [Fact]
    public async Task GetDetails_WithCustomerId_ShouldReturnGetCustomerDetails()
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

        _queryBus
            .Setup(m => m.Send(It.IsAny<GetCustomerDetails>()))
            .ReturnsAsync(expectedData);

        // When
        var response = await _customersController.GetDetails();

        // Then
        response.Should().BeOfType<OkObjectResult>()
            .Subject.Value.Should().BeOfType<ApiResponse<CustomerDetails>>()
            .Subject.Data.Should().BeEquivalentTo(expectedData);
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

        _queryBus
            .Setup(m => m.Send(It.IsAny<GetCreditLimit>()))
            .ReturnsAsync(expectedData);

        // When
        var response = await _customersController.GetCustomerCreditLimit(customerId);

        // Then
        response.Should().BeOfType<OkObjectResult>()
            .Subject.Value.Should().BeOfType<ApiResponse<CreditLimitModel>>()
            .Subject.Data.Should().BeEquivalentTo(expectedData);
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
                "event data"
            ),
            new CustomerEventHistory(
                Guid.NewGuid(),
                customerId,
                typeof(CustomerUpdated).Name,
                "event data"
            )
        };

        _queryBus
            .Setup(m => m.Send(It.IsAny<GetCustomerEventHistory>()))
            .ReturnsAsync(expectedData);

        // When
        var response = await _customersController.ListHistory(customerId);

        // Then
        response.Should().BeOfType<OkObjectResult>()
            .Subject.Value.Should().BeOfType<ApiResponse<IList<CustomerEventHistory>>>()
            .Subject.Data.Should().BeEquivalentTo(expectedData);
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

        _commandBus
            .Setup(m => m.Send(It.IsAny<RegisterCustomer>()));

        // When
        var response = await _customersController.Register(request);

        // Then
        response.Should().BeOfType<OkObjectResult>();
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

        _commandBus
            .Setup(m => m.Send(It.IsAny<RegisterCustomer>()));

        // When
        var response = await _customersController.UpdateInformation(customerId, request);

        // Then
        response.Should().BeOfType<OkObjectResult>();
    }

    private Mock<ICommandBus> _commandBus = new();
    private Mock<IQueryBus> _queryBus = new();
    private CustomersController _customersController;
    private Mock<ITokenRequester> _tokenRequester = new();
}