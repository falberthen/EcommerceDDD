namespace EcommerceDDD.QuoteManagement.API.Controllers.Requests;

public record class ConfirmQuoteRequest
{
    [Required(ErrorMessage = "The {0} field is required.")]
    public Guid OrderId { get; init; }
}
