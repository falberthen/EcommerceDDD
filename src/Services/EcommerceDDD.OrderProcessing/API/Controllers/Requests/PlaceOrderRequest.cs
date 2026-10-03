namespace EcommerceDDD.OrderProcessing.API.Controllers.Requests;

public record class PlaceOrderRequest
{
    [Required(ErrorMessage = "The {0} field is required.")]
    public Guid QuoteId { get; init; }
}