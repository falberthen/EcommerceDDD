namespace EcommerceDDD.Products.API.Controllers.Requests;

public record class ProductStockAvailabilityRequest
{
    [Required(ErrorMessage = "The {0} field is required.")]
    [MinLength(1, ErrorMessage = "At least one ProductId is required.")]
    public Guid[] ProductIds { get; init; }
}