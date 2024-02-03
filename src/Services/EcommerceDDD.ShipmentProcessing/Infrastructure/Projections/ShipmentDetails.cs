using PackageShipped = EcommerceDDD.ShipmentProcessing.Domain.Events.PackageShipped;

namespace EcommerceDDD.ShipmentProcessing.Infrastructure.Projections;

public class ShipmentDetails
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public IReadOnlyList<ProductItemDetails> ProductItems { get; set; } = default!;
    public DateTime? CreatedAt { get; set; }
    public DateTime? ShippedAt { get; set; }
    public ShipmentStatus Status { get; set; }

    internal void Apply(ShipmentCreated @event)
    {
        var productItems = @event.ProductItems.Select(c =>
            new ProductItemDetails(
                c.ProductId,
                c.Quantity)
            ).ToList();

        Id = @event.ShipmentId;
        OrderId = @event.OrderId;
        ProductItems = productItems;
        CreatedAt = @event.Timestamp;
        Status = ShipmentStatus.Pending;
    }

    internal void Apply(PackageShipped @event)
    {
        ShippedAt = @event.Timestamp;
        Status = ShipmentStatus.Shipped;
    }
}