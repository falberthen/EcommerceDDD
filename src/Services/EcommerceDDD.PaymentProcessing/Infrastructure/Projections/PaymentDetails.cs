using PaymentCompleted = EcommerceDDD.PaymentProcessing.Domain.Events.PaymentCompleted;

namespace EcommerceDDD.PaymentProcessing.Infrastructure.Projections;

public class PaymentDetails
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public Guid OrderId { get; set; }
    public PaymentStatus Status { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? CanceledAt { get; set; }
    public decimal TotalAmount { get; set; }
    public string CurrencyCode { get; set; }
	public IReadOnlyList<ProductItemDetails> ProductItems { get; set; } = default!;

	internal void Apply(PaymentCreated @event)
    {
        Id = @event.PaymentId;
        CustomerId = @event.CustomerId;
        OrderId = @event.OrderId;
        TotalAmount = @event.TotalAmount;
        CurrencyCode = @event.CurrencyCode;
        Status = PaymentStatus.Pending;

		var productItems = @event.ProductItems.Select(c =>
			new ProductItemDetails(
				c.ProductId,
				c.Quantity)
			).ToList();
		ProductItems = productItems;
	}

	internal void Apply(PaymentCompleted @event)
    {
        CompletedAt = @event.Timestamp;
        Status = PaymentStatus.Completed;
    }

    internal void Apply(PaymentCanceled @event)
    {
        CanceledAt = @event.Timestamp;
        Status = PaymentStatus.Canceled;
    }
}