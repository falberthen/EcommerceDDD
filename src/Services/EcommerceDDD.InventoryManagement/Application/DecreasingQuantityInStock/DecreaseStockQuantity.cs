namespace EcommerceDDD.InventoryManagement.Application.DecreasingQuantityInStock;

public record class DecreaseStockQuantity : ICommand
{
	public ProductId ProductId { get; private set; }
	public int QuantityDecreased { get; private set; }
	public Guid OrderId { get; private set; }

	public static DecreaseStockQuantity Create(
	   ProductId productId,
	   int quantityDecreased,
	   Guid orderId)
	{
		if (productId is null)
			throw new ArgumentNullException(nameof(productId));
		if (quantityDecreased <= 0)
			throw new ArgumentNullException(nameof(quantityDecreased));

		if (orderId == Guid.Empty)
			throw new ArgumentNullException(nameof(orderId));

		return new DecreaseStockQuantity(productId, quantityDecreased, orderId);
	}

	private DecreaseStockQuantity(ProductId productId, int quantityDecreased, Guid orderId)
	{
		ProductId = productId;
		QuantityDecreased = quantityDecreased;
		OrderId = orderId;
	}
}