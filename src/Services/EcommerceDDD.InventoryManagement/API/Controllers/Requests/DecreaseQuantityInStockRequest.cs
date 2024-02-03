namespace EcommerceDDD.InventoryManagement.API.Controllers.Requests;

public record class DecreaseQuantityInStockRequest
{
    [Required(ErrorMessage = "The {0} field is required.")]
    public Guid ProductId { get; init; }

    [Range(0, int.MaxValue, ErrorMessage = "The value must be greater than 0")]
    public int DecreasedQuantity { get; init; }
}