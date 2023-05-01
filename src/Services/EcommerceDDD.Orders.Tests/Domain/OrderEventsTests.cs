namespace EcommerceDDD.Orders.Tests.Domain;

public class OrderEventsTests
{
    [Fact]
    public void PlacingOrder_WithOrderData_ReturnsOrderPlacedEvent()
    {
        // Given        
        var quoteItems = new List<ProductItemData>() {
            new ProductItemData() {
                ProductId = _productId,
                ProductName = productName,
                Quantity = 1,
                UnitPrice = productPrice
            }
        };

        var orderData = new OrderData(_quoteId, _customerId, quoteItems, currency);

        // When
        var order = Order.Create(orderData);

        // Then
        var @event = order.GetUncommittedEvents().LastOrDefault() as OrderPlaced;
        Assert.NotNull(@event);
        @event.Should().BeOfType<OrderPlaced>();
    }

    [Fact]
    public void CancelingOrder_WithCancellationReason_ReturnsOrderCanceledEvent()
    {
        // Given        
        var quoteItems = new List<ProductItemData>() {
            new ProductItemData() {
                ProductId = _productId,
                ProductName = productName,
                Quantity = 1,
                UnitPrice = productPrice
            }
        };

        var orderData = new OrderData(_quoteId, _customerId, quoteItems, currency);
        var order = Order.Create(orderData);

        // When
        order.Cancel(OrderCancellationReason.CanceledByUser);

        // Then
        var @event = order.GetUncommittedEvents().LastOrDefault() as OrderCanceled;
        Assert.NotNull(@event);
        @event.Should().BeOfType<OrderCanceled>();
    }

    [Fact]
    public void RecordingPaymentToOrder_WithPaymentId_ReturnsOrderPaidEvent()
    {
        // Given
        var totalPaid = Money.Of(100, Currency.USDollar.Code);

        var quoteItems = new List<ProductItemData>() {
            new ProductItemData() {
                ProductId = _productId,
                ProductName = productName,
                Quantity = 1,
                UnitPrice = productPrice
            }
        };

        var orderData = new OrderData(_quoteId, _customerId, quoteItems, currency);
        var order = Order.Create(orderData);

        // When
        order.RecordPayment(_paymentId, totalPaid);

        // Then
        var @event = order.GetUncommittedEvents().LastOrDefault() as OrderPaid;
        Assert.NotNull(@event);
        @event.Should().BeOfType<OrderPaid>();
    }

    [Fact]
    public void CompletingOrder_WithShipmentId_ReturnsOrderCompletedEvent()
    {
        // Given
        var totalPaid = Money.Of(100, Currency.USDollar.Code);

        var quoteItems = new List<ProductItemData>() {
            new ProductItemData() {
                ProductId = _productId,
                ProductName = productName,
                Quantity = 1,
                UnitPrice = productPrice
            }
        };

        var orderData = new OrderData(_quoteId, _customerId, quoteItems, currency);

        var order = Order.Create(orderData);
        order.RecordPayment(_paymentId, totalPaid);
        order.RecordShippedEvent();

        // When
        order.Complete(_shipmentId);

        // Then
        var @event = order.GetUncommittedEvents().LastOrDefault() as OrderCompleted;
        Assert.NotNull(@event);
        @event.Should().BeOfType<OrderCompleted>();
    }

    const string productName = "Product XYZ";
    private CustomerId _customerId = CustomerId.Of(Guid.NewGuid());
    private QuoteId _quoteId = QuoteId.Of(Guid.NewGuid());
    private ProductId _productId = ProductId.Of(Guid.NewGuid());
    private PaymentId _paymentId = PaymentId.Of(Guid.NewGuid());
    private ShipmentId _shipmentId = ShipmentId.Of(Guid.NewGuid());
    private Currency currency = Currency.OfCode(Currency.USDollar.Code);
    private Money productPrice = Money.Of(15, Currency.USDollar.Code);
}