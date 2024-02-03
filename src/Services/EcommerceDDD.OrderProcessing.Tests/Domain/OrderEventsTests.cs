using NSubstitute.ReceivedExtensions;

namespace EcommerceDDD.OrderProcessing.Tests.Domain;

public class OrderEventsTests
{
    [Fact]
    public void Place_WithOrderData_ShouldApplyOrderPlacedEvent()
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

        var orderData = new OrderData(_customerId, _quoteId, currency, quoteItems);

        // When
        var order = Order.Place(orderData);

        // Then
        var @event = order.GetUncommittedEvents().LastOrDefault() as OrderPlaced;
        Assert.NotNull(@event);
        @event.Should().BeOfType<OrderPlaced>();
    }

    [Fact]
    public void Process_WithQuoteItems_ShouldApplyOrderProcessedEvent()
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

        var orderData = new OrderData(_customerId, _quoteId, currency, quoteItems);
        var order = Order.Place(orderData);

        // When
        order.Process(orderData);

        // Then
        var @event = order.GetUncommittedEvents().LastOrDefault() as OrderProcessed;
        Assert.NotNull(@event);
        @event.Should().BeOfType<OrderProcessed>();
    }

    [Fact]
    public void RecordPayment_WithPaymentId_ShouldApplyOrderPaidEvent()
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

        var orderData = new OrderData(_customerId, _quoteId, currency, quoteItems);
        var order = Order.Place(orderData);
        order.Process(orderData);
        
        // When
        order.RecordPayment(_paymentId, totalPaid);

        // Then
        var @event = order.GetUncommittedEvents().LastOrDefault() as OrderPaid;
        Assert.NotNull(@event);
        @event.Should().BeOfType<OrderPaid>();
    }

    [Fact]
    public void RecordShipment_WithShipmentId_ShouldApplyOrderShippedEvent()
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

        var orderData = new OrderData(_customerId, _quoteId, currency, quoteItems);
        var order = Order.Place(orderData);
        order.Process(orderData);
        order.RecordPayment(_paymentId, totalPaid);

        // When
        order.RecordShipment(_shipmentId);

        // Then
        var @event = order.GetUncommittedEvents().LastOrDefault() as OrderShipped;
        Assert.NotNull(@event);
        @event.Should().BeOfType<OrderShipped>();
    }

    [Fact]
    public void CompletingOrder_WithAllFlowSteps_ShouldApplyOrderCompletedEvent()
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

        var orderData = new OrderData(_customerId, _quoteId, currency, quoteItems);

        var order = Order.Place(orderData);
        order.Process(orderData);
        order.RecordPayment(_paymentId, totalPaid);
        order.RecordShipment(_shipmentId);

        // When
        order.Complete(_shipmentId);

        // Then
        var @event = order.GetUncommittedEvents().LastOrDefault() as OrderCompleted;
        Assert.NotNull(@event);
        @event.Should().BeOfType<OrderCompleted>();
    }

    [Fact]
    public void Cancel_WithCancellationReason_ShouldApplyOrderCanceledEvent()
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

        var orderData = new OrderData(_customerId, _quoteId, currency, quoteItems);
        var order = Order.Place(orderData);

        // When
        order.Cancel(OrderCancellationReason.CanceledByCustomer);

        // Then
        var @event = order.GetUncommittedEvents().LastOrDefault() as OrderCanceled;
        Assert.NotNull(@event);
        @event.Should().BeOfType<OrderCanceled>();
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