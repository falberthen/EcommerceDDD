namespace EcommerceDDD.InventoryManagement.API.Controllers.Requests;

public record class CheckProductsInStockRequest
{
    [Required(ErrorMessage = "The {0} field is required.")]
    public Guid[] ProductIds { get; init; }
}