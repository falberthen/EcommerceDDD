using EcommerceDDD.Core.Testing;
using EcommerceDDD.Shipments.Application.ShippingPackage;
using EcommerceDDD.Shipments.Domain;
using EcommerceDDD.IntegrationServices.Products;
using EcommerceDDD.IntegrationServices;
using Microsoft.Extensions.Options;
using EcommerceDDD.Core.EventBus;
using static EcommerceDDD.IntegrationServices.Products.Responses.ProductStockAvailabilityResponse;

namespace EcommerceDDD.Shipments.Tests.Application;

public class ShipPackageHandlerTests
{
    [Fact]
    public async Task ShipPackage_WithCommand_ShouldShipPackage()
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

        // When
        await shipPackageHandler.Handle(shipPackage, CancellationToken.None);

        // Then
        var shipment = shipmentWriteRepository.AggregateStream.First().Aggregate;
        Assert.NotNull(shipment);
        shipment.OrderId.Should().Be(orderId);
        shipment.ProductItems.Count().Should().Be(productItems.Count());
        shipment.DeliveredAt.Should().Be(null);
        shipment.ShippedAt.Should().NotBe(null);        
        shipment.Status.Should().Be(ShipmentStatus.Shipped);
    }

    private Mock<IProductsService> _productsService = new();
    private Mock<IEventProducer> _eventProducer = new();
}