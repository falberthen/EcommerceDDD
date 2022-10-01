using System.ComponentModel.DataAnnotations;

namespace EcommerceDDD.Quotes.API.Controllers.Requests;

public record class AddQuoteItemRequest
{
    [Required(ErrorMessage = "The {0} field is required.")]
    public Guid ProductId { get; init; }

    [Required(ErrorMessage = "The {0} field is required.")]
    public int Quantity { get; init; }

    [Required(ErrorMessage = "The {0} field is required.")]
    public string CurrencyCode { get; init; }
}