using EcommerceDDD.Core.EventBus;
using EcommerceDDD.Core.Testing;
using EcommerceDDD.Shipments.Domain;
using EcommerceDDD.Shipments.Domain.Events;
using EcommerceDDD.Shipments.Domain.Commands;
using EcommerceDDD.Shipments.Application.ShippingPackage;
using EcommerceDDD.Shipments.Application.DeliveringPackage;
using EcommerceDDD.Core.Infrastructure.Outbox.Services;

namespace EcommerceDDD.Shipments.Tests.Application;

public class PackageShippedHandlerTests
{
    [Fact]
    public async Task PackageShipped_WithDomainEvent_ShouldDeliverPackage()
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

        var shipPackage = ShipPackage.Create(orderId, productItems);
        var shipPackageHandler = new ShipPackageHandler(_availabilityChecker.Object, shipmentWriteRepository, _outboxMessageService.Object);
        await shipPackageHandler.Handle(shipPackage, CancellationToken.None);

        var shipment = shipmentWriteRepository.AggregateStream.First().Aggregate;
        var @event = shipment.GetUncommittedEvents().LastOrDefault() as PackageShipped;
        Assert.NotNull(@event);

        var packageShippedHandler = new PackageShippedHandler(shipmentWriteRepository);

        // When
        await packageShippedHandler.Handle(@event!, CancellationToken.None);

        // Then
        Assert.NotNull(shipment);
        shipment.OrderId.Should().Be(orderId);
        shipment.ProductItems.Count().Should().Be(productItems.Count());
        shipment.ShippedAt.Should().NotBe(null);
        shipment.DeliveredAt.Should().NotBe(null);
        shipment.Status.Should().Be(ShipmentStatus.Delivered);
    }

    private Mock<IOutboxMessageService> _outboxMessageService = new();
    private Mock<IProductAvailabilityChecker> _availabilityChecker = new();
}