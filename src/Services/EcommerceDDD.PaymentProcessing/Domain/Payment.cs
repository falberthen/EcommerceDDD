using PaymentCompleted = EcommerceDDD.PaymentProcessing.Domain.Events.PaymentCompleted;

namespace EcommerceDDD.PaymentProcessing.Domain;

public class Payment : AggregateRoot<PaymentId>
{
    public CustomerId CustomerId { get; private set; }
    public OrderId OrderId { get; private set; }
	public IReadOnlyList<ProductItem> ProductItems { get; set; } = default!;
	public PaymentStatus Status { get; private set; }
    public DateTime? CreatedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public DateTime? CanceledAt { get; private set; }
    public Money TotalAmount { get; private set; }

    public static Payment Create(PaymentData paymentData)
    {
        var (CustomerId, OrderId, TotalAmount, ProductItems) = paymentData
            ?? throw new ArgumentNullException(nameof(paymentData));

        if (CustomerId is null)
            throw new BusinessRuleException("The customer Id is required.");

        if (OrderId is null)
            throw new BusinessRuleException("The order Id is required.");
    
        if (TotalAmount is null)
            throw new BusinessRuleException("The total amount is required.");

		if (ProductItems is null || paymentData.ProductItems.Count == 0)
			throw new BusinessRuleException("There are no products to pay.");

		return new Payment(paymentData);
    }

    public void Complete()
    {
        if (Status != PaymentStatus.Pending)
            throw new BusinessRuleException($"Payment cannot be completed when '{Status}'");

        var @event = PaymentCompleted.Create(Id.Value);

        AppendEvent(@event);
        Apply(@event);
    }

    public void Cancel(PaymentCancellationReason paymentCancellationReason)
    {
        if (Status == PaymentStatus.Canceled)
            throw new BusinessRuleException($"Payment cannot be canceled when '{Status}'");

        var @event = PaymentCanceled.Create(
            Id.Value,            
            paymentCancellationReason);

        AppendEvent(@event);
        Apply(@event);
    }

    private void Apply(PaymentCreated @event)
    {
        Status = PaymentStatus.Pending;
        Id = PaymentId.Of(@event.PaymentId);
        CustomerId = CustomerId.Of(@event.CustomerId);
        OrderId = OrderId.Of(@event.OrderId);
		ProductItems = @event.ProductItems.Select(p =>
			new ProductItem(
				ProductId.Of(p.ProductId),
				p.Quantity)).ToList();
		TotalAmount = Money.Of(
            @event.TotalAmount,
            @event.CurrencyCode);
        CreatedAt = @event.Timestamp;
    }

    private void Apply(PaymentCompleted @event)
    {
        Status = PaymentStatus.Completed;
        CompletedAt = @event.Timestamp;
    }

    private void Apply(PaymentCanceled @event)
    {
        Status = PaymentStatus.Canceled;
        CanceledAt = @event.Timestamp;
    }

    private Payment(PaymentData paymentData)
    {       
        var @event = PaymentCreated.Create(
            Guid.NewGuid(),
            paymentData.CustomerId.Value,
            paymentData.OrderId.Value,
            paymentData.TotalAmount.Amount,
            paymentData.TotalAmount.Currency.Code,
			paymentData.ProductItems);

        AppendEvent(@event);
        Apply(@event);
    }

    private Payment() {}
}
