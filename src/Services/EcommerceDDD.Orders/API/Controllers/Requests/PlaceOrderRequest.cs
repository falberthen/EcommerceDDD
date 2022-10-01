using System.ComponentModel.DataAnnotations;

namespace EcommerceDDD.Orders.API.Controllers.Requests;

public record class PlaceOrderRequest
{
    [Required(ErrorMessage = "The {0} field is required.")]
    public Guid QuoteId { get; init; }

    [Required(ErrorMessage = "The {0} field is required.")]
    public Guid CustomerId { get; init; }

    [Required(ErrorMessage = "The {0} field is required.")]
    public List<QuoteItemRequest> Items { get; init; }

    [Required(ErrorMessage = "The {0} field is required.")]
    public string CurrencyCode { get; init; }
}

public record class QuoteItemRequest(
    Guid ProductId,
    int Quantity);
