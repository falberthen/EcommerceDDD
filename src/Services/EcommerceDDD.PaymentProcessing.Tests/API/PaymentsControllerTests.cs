namespace EcommerceDDD.PaymentProcessing.Tests;

public class PaymentsControllerTests
{    
    public PaymentsControllerTests()
    {
        _paymentsController = new PaymentsController(
            _commandBus,
            _queryBus);
    }

    [Fact]
    public async Task RequestCreate_WithRequestPayment_ShouldCreatePayment()
    {
        // Given
        Guid customerId = Guid.NewGuid();
        Guid orderId = Guid.NewGuid();
		Guid productId = Guid.NewGuid();

		await _commandBus.SendAsync(Arg.Any<RequestPayment>(), CancellationToken.None);

        var request = new PaymentRequest()
        {
            CurrencyCode = Currency.USDollar.Code,
            CustomerId = customerId,
            OrderId = orderId,
            TotalAmount = 10m,
			ProductItems = new List<ProductItemRequest>
			{
				new ProductItemRequest(
					productId,
					"Product X",
					10m,
					1
				)
			}
		};

        // When
        var response = await _paymentsController.RequestCreate(request, 
            Arg.Any<CancellationToken>());

        // Then
		Assert.IsType<OkObjectResult>(response);
    }

    private ICommandBus _commandBus = Substitute.For<ICommandBus>();
    private IQueryBus _queryBus = Substitute.For<IQueryBus>();
    private PaymentsController _paymentsController;
}