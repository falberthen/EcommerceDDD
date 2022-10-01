using EcommerceDDD.Core.CQRS.CommandHandling;

namespace EcommerceDDD.Shipments.Domain.Commands;

public record class ShipPackage : ICommand
{
    public OrderId OrderId { get; private set; }
    public IReadOnlyList<ProductItem> ProductItems { get; private set; }

    public static ShipPackage Create(
        OrderId orderId,
        IReadOnlyList<ProductItem> productItems)
    {
        if (orderId is null)
            throw new ArgumentNullException(nameof(OrderId));
        if (productItems.Count == 0)
            throw new ArgumentOutOfRangeException(nameof(productItems));

        return new ShipPackage(orderId, productItems);
    }

    private ShipPackage(
        OrderId orderId,
        IReadOnlyList<ProductItem> productItems)
    {
        OrderId = orderId;
        ProductItems = productItems;
    }
}