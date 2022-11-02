using EcommerceDDD.Core.Testing;
using EcommerceDDD.Core.EventBus;
using EcommerceDDD.Shipments.Domain;
using EcommerceDDD.Shipments.Domain.Commands;
using EcommerceDDD.Core.Infrastructure.Integration;
using EcommerceDDD.Shipments.Application.ShippingPackage;
using EcommerceDDD.Core.Infrastructure.Outbox.Services;

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

        var response = new IntegrationHttpResponse<List<ProductInStockViewModel>>()
        {
            Success = true,
            Data = new List<ProductInStockViewModel>()
            {
                new ProductInStockViewModel(productItems[0].ProductId.Value, 10),
                new ProductInStockViewModel(productItems[1].ProductId.Value, 15),
                new ProductInStockViewModel(productItems[2].ProductId.Value, 15),
            }
        };
        _availabilityChecker.Setup(p => p.EnsureProductsInStock(It.IsAny<IReadOnlyList<ProductItem>>()))
            .Returns(Task.FromResult(true));

        var shipPackage = ShipPackage.Create(orderId, productItems);
        var shipPackageHandler = new ShipPackageHandler(_availabilityChecker.Object, shipmentWriteRepository, _outboxMessageService.Object);

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

    private Mock<IOutboxMessageService> _outboxMessageService = new();
    private Mock<IProductAvailabilityChecker> _availabilityChecker = new();
}