namespace EcommerceDDD.InventoryManagement.Application.IncreaseQuantityInStock;

public record class IncreaseStockQuantity : ICommand
{
	public ProductId ProductId { get; private set; }
	public int QuantityIncreased { get; private set; }
	public Guid OrderId { get; private set; }

	public static IncreaseStockQuantity Create(
	   ProductId productId,
	   int quantityIncreased,
	   Guid orderId)
	{
		if (productId is null)
			throw new ArgumentNullException(nameof(productId));
		if (quantityIncreased < 1)
			throw new ArgumentNullException(nameof(quantityIncreased));

		if (orderId == Guid.Empty)
			throw new ArgumentNullException(nameof(orderId));

		return new IncreaseStockQuantity(productId, quantityIncreased, orderId);
	}

	private IncreaseStockQuantity(ProductId productId, int quantityIncreased, Guid orderId)
	{
		ProductId = productId;
		QuantityIncreased = quantityIncreased;
		OrderId = orderId;
	}
}