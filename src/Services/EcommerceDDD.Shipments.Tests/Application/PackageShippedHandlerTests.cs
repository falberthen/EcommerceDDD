using EcommerceDDD.Core.Testing;
using EcommerceDDD.Shipments.Domain;
using EcommerceDDD.Shipments.Domain.Events;
using EcommerceDDD.Shipments.Application.ShippingPackage;
using EcommerceDDD.Shipments.Application.DeliveringPackage;
using EcommerceDDD.IntegrationServices.Products;
using EcommerceDDD.IntegrationServices;
using Microsoft.Extensions.Options;
using EcommerceDDD.Core.Domain;
using EcommerceDDD.Core.EventBus;
using static EcommerceDDD.IntegrationServices.Products.Responses.ProductStockAvailabilityResponse;
using EcommerceDDD.Core.Persistence;

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

        var options = new Mock<IOptions<IntegrationServicesSettings>>();
        options.Setup(p => p.Value)
            .Returns(new IntegrationServicesSettings() { ApiGatewayBaseUrl = "http://url" });

        var productsInStock = new List<ProductInStockViewModel>()
        {
            new ProductInStockViewModel(productItems[0].ProductId.Value, 10),
            new ProductInStockViewModel(productItems[1].ProductId.Value, 15),
            new ProductInStockViewModel(productItems[2].ProductId.Value, 15),
        };

        _productsService.Setup(p => p.CheckProducStockAvailability(It.IsAny<string>(), It.IsAny<Guid[]>()))
            .Returns(Task.FromResult(productsInStock));

        var shipPackage = new ShipPackage(orderId, productItems);
        var shipPackageHandler = new ShipPackageHandler(_productsService.Object, _eventProducer.Object,
            shipmentWriteRepository, options.Object);

        await shipPackageHandler.Handle(shipPackage, CancellationToken.None);

        var shipment = shipmentWriteRepository.AggregateStream.First().Aggregate;
        var @event = shipment.GetUncommittedEvents().LastOrDefault() as PackageShipped;
        Assert.NotNull(@event);

        var serviceProvider = DummyServiceProvider.Setup();
        serviceProvider
            .Setup(x => x.GetService(typeof(IEventStoreRepository<Shipment>)))
            .Returns(shipmentWriteRepository);

        var packageShippedHandler = new PackageShippedHandler(serviceProvider.Object);
        var domainNotification = new DomainEventNotification<PackageShipped>(@event!);

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

    private Mock<IProductsService> _productsService = new();
    private Mock<IEventProducer> _eventProducer = new();
}