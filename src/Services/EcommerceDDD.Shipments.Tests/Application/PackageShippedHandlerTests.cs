using EcommerceDDD.Core.Domain;
using EcommerceDDD.Core.EventBus;
using EcommerceDDD.Core.Testing;
using EcommerceDDD.Shipments.Domain;
using EcommerceDDD.Shipments.Domain.Events;
using EcommerceDDD.Shipments.Domain.Commands;
using EcommerceDDD.Core.Infrastructure.Integration;
using EcommerceDDD.Shipments.Application.ShippingPackage;
using EcommerceDDD.Shipments.Application.DeliveringPackage;

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
        var shipPackageHandler = new ShipPackageHandler(_eventProducer.Object, _availabilityChecker.Object, shipmentWriteRepository);
        await shipPackageHandler.Handle(shipPackage, CancellationToken.None);

        var shipment = shipmentWriteRepository.AggregateStream.First().Aggregate;
        var @event = shipment.GetUncommittedEvents().LastOrDefault() as PackageShipped;
        Assert.NotNull(@event);

        var packageShippedHandler = new PackageShippedHandler(shipmentWriteRepository);
        var domainNotification = new DomainNotification<PackageShipped>(@event!);

        // When
        await packageShippedHandler.Handle(domainNotification, CancellationToken.None);

        // Then
        Assert.NotNull(shipment);
        shipment.OrderId.Should().Be(orderId);
        shipment.ProductItems.Count().Should().Be(productItems.Count());
        shipment.ShippedAt.Should().NotBe(null);
        shipment.DeliveredAt.Should().NotBe(null);
        shipment.Status.Should().Be(ShipmentStatus.Delivered);
    }
    
    private Mock<IEventProducer> _eventProducer = new();
    private Mock<IProductAvailabilityChecker> _availabilityChecker = new();
}