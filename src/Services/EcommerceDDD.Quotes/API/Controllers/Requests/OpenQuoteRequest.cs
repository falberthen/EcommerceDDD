namespace EcommerceDDD.Quotes.API.Controllers.Requests;

public record class OpenQuoteRequest
{
    [Required(ErrorMessage = "The {0} field is required.")]
    public Guid CustomerId { get; init; }
}