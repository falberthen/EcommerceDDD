namespace EcommerceDDD.PaymentProcessing.Domain.Events;

public record PaymentCreated : DomainEvent
{
    public Guid PaymentId { get; private set; }
    public Guid CustomerId { get; private set; }
    public Guid OrderId { get; private set; }
    public decimal TotalAmount { get; private set; }
    public string CurrencyCode { get; private set; }
	public IReadOnlyList<ProductItemDetails> ProductItems { get; private set; }

	public static PaymentCreated Create(
        Guid paymentId,
        Guid customerId,
        Guid orderId,
        decimal totalAmount,
        string currencyCode,
		IReadOnlyList<ProductItem> productItems)
	{
		if (paymentId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(paymentId));
        if (customerId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(customerId));
        if (orderId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(orderId));
        if (totalAmount <= 0)
            throw new ArgumentOutOfRangeException(nameof(totalAmount));
        if (string.IsNullOrEmpty(currencyCode))
            throw new ArgumentNullException(nameof(currencyCode));

		var items = productItems.Select(ol => new ProductItemDetails(
		   ol.ProductId.Value,
		   ol.Quantity)).ToList();

		return new PaymentCreated(
            paymentId,
            customerId,
            orderId, 
            totalAmount,
            currencyCode,
			items);
    }

    private PaymentCreated(
        Guid paymentId,
        Guid customerId,
        Guid orderId,
        decimal totalAmount,
        string currencyCode,
		IReadOnlyList<ProductItemDetails> productItems)
	{
		PaymentId = paymentId;
        CustomerId = customerId;
        OrderId = orderId;
        TotalAmount = totalAmount;
        CurrencyCode = currencyCode;
		ProductItems = productItems;
	}
}

public record ProductItemDetails(Guid ProductId, int Quantity);