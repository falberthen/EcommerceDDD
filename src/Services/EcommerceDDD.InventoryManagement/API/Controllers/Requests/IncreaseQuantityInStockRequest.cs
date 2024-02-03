namespace EcommerceDDD.InventoryManagement.API.Controllers.Requests;

public record class IncreaseQuantityInStockRequest
{
    [Required(ErrorMessage = "The {0} field is required.")]
    public Guid ProductId { get; init; }

    [Range(1, int.MaxValue, ErrorMessage = "The value must be at least 1")]
    public int IncreasedQuantity { get; init; }
}