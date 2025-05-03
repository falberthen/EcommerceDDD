namespace EcommerceDDD.PaymentProcessing.Domain.Commands;

public record class RequestPayment : ICommand
{
    public CustomerId CustomerId { get; private set; }
    public OrderId OrderId { get; private set; }
    public Money TotalAmount { get; private set; }
    public Currency Currency { get; private set; }
	public IReadOnlyList<ProductItem> ProductItems { get; private set; }

	public static RequestPayment Create(
        CustomerId customerId,
        OrderId orderId,
        Money totalPrice,
        Currency currency,
		IReadOnlyList<ProductItem> productItems)
    {
        if (customerId is null)
            throw new ArgumentNullException(nameof(customerId));
        if (orderId is null)
            throw new ArgumentNullException(nameof(orderId));
        if (totalPrice is null)
            throw new ArgumentNullException(nameof(totalPrice));
        if (currency is null)
            throw new ArgumentNullException(nameof(currency));
		if (productItems.Count == 0)
			throw new ArgumentOutOfRangeException(nameof(productItems));

		return new RequestPayment(customerId, orderId, totalPrice, currency, productItems);
    }

    private RequestPayment(
        CustomerId customerId,
        OrderId orderId,
        Money totalAmount,
        Currency currency,
		IReadOnlyList<ProductItem> productItems)
    {
        CustomerId = customerId;
        OrderId = orderId;
        TotalAmount = totalAmount;
        Currency = currency;
		ProductItems = productItems;
	}
}