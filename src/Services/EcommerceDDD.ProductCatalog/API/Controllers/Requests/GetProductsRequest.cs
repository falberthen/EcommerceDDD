namespace EcommerceDDD.ProductCatalog.API.Controllers.Requests;

public record class GetProductsRequest
{    
    public Guid[]? ProductIds { get; init; }

    [Required(ErrorMessage = "The {0} field is required.")]
    public string CurrencyCode { get; init; }
}