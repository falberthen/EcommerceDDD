namespace EcommerceDDD.Payments.Tests;

public class PaymentsControllerTests
{    
    public PaymentsControllerTests()
    {
        _paymentsController = new PaymentsController(
            _commandBus.Object,
            _queryBus.Object);
    }

    [Fact]
    public async Task RequestCreate_WithRequestPayment_ShouldCreatePayment()
    {
        // Given
        Guid customerId = Guid.NewGuid();
        Guid orderId = Guid.NewGuid();

        _commandBus
            .Setup(m => m.Send(It.IsAny<RequestPayment>()));

        var request = new PaymentRequest()
        {
            CurrencyCode = Currency.USDollar.Code,
            CustomerId = customerId,
            OrderId = orderId,
            TotalAmount = 10m
        };

        // When
        var response = await _paymentsController.RequestCreate(request);

        // Then
        response.Should().BeOfType<OkObjectResult>();
    }

    private Mock<ICommandBus> _commandBus = new();
    private Mock<IQueryBus> _queryBus = new();
    private PaymentsController _paymentsController;
}