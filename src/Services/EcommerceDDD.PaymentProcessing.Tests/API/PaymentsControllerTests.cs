namespace EcommerceDDD.PaymentProcessing.Tests;

public class PaymentsControllerTests
{    
    public PaymentsControllerTests()
    {
        _paymentsController = new PaymentsController(_bus);
    }

    [Fact]
    public async Task RequestCreate_WithRequestPayment_ShouldCreatePayment()
    {
        // Given
        Guid customerId = Guid.NewGuid();
        Guid orderId = Guid.NewGuid();
		Guid productId = Guid.NewGuid();

		_bus.InvokeAsync<Result>(Arg.Any<RequestPayment>(), Arg.Any<CancellationToken>(), Arg.Any<TimeSpan?>())
			.Returns(Task.FromResult(Result.Ok()));

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
        var response = await _paymentsController.RequestCreate(request, CancellationToken.None);

        // Then
		Assert.IsType<OkResult>(response);
    }

    private IMessageBus _bus = Substitute.For<IMessageBus>();
    private PaymentsController _paymentsController;
}