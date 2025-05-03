namespace EcommerceDDD.PaymentProcessing.API.Controllers.Requests;

public record class PaymentRequest
{
    [Required(ErrorMessage = "The {0} field is required.")]
    public Guid CustomerId { get; init; }

    [Required(ErrorMessage = "The {0} field is required.")]
    public Guid OrderId { get; init; }

    [Required(ErrorMessage = "The {0} field is required.")]
    public decimal TotalAmount { get; init; }

    [Required(ErrorMessage = "The {0} field is required.")]
    public string CurrencyCode { get; init; }

	[Required(ErrorMessage = "The {0} field is required.")]
	public IReadOnlyList<ProductItemRequest> ProductItems { get; init; }
}

public record ProductItemRequest(
	Guid ProductId,
	string ProductName,
	decimal UnitPrice,
	int Quantity);