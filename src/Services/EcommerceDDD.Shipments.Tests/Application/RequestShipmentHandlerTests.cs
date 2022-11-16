using EcommerceDDD.Core.Testing;
using EcommerceDDD.Shipments.Domain;
using EcommerceDDD.Shipments.Domain.Commands;
using EcommerceDDD.Core.Infrastructure.Outbox.Services;
using EcommerceDDD.Shipments.Application.RequestingShipment;
using EcommerceDDD.Core.CQRS.CommandHandling;

namespace EcommerceDDD.Shipments.Tests.Application;

public class RequestShipmentHandlerTests
{
    [Fact]
    public async Task RequestShipment_WithCommand_ShouldCreatePendingShipment()
    {
        // Given
        var orderId = OrderId.Of(Guid.NewGuid());
        var productItems = new List<ProductItem>() {
            new ProductItem(ProductId.Of(Guid.NewGuid()), 5),
            new ProductItem(ProductId.Of(Guid.NewGuid()), 1),
            new ProductItem(ProductId.Of(Guid.NewGuid()), 1)
        };

        var shipmentWriteRepository = new DummyEventStoreRepository<Shipment>();

        _availabilityChecker.Setup(p => p.EnsureProductsInStock(It.IsAny<IReadOnlyList<ProductItem>>()))
            .Returns(Task.FromResult(true));

        var requestShipment = RequestShipment.Create(orderId, productItems);
        var requestShipmentHandler = new RequestShipmentHandler(_commandBus.Object, _availabilityChecker.Object, shipmentWriteRepository, _outboxMessageService.Object);

        // When
        await requestShipmentHandler.Handle(requestShipment, CancellationToken.None);

        // Then
        var shipment = shipmentWriteRepository.AggregateStream.First().Aggregate;
        Assert.NotNull(shipment);
        shipment.OrderId.Should().Be(orderId);
        shipment.ProductItems.Count().Should().Be(productItems.Count());
        shipment.CreatedAt.Should().NotBe(null);
        shipment.ShippedAt.Should().Be(null);
        shipment.Status.Should().Be(ShipmentStatus.Pending);
    }

    private Mock<ICommandBus> _commandBus = new();
    private Mock<IOutboxMessageService> _outboxMessageService = new();
    private Mock<IProductAvailabilityChecker> _availabilityChecker = new();
}