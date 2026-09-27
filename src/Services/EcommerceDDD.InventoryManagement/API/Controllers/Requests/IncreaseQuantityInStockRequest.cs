namespace EcommerceDDD.InventoryManagement.API.Controllers.Requests;

public record class IncreaseQuantityInStockRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "The value must be at least 1")]
    public int IncreasedQuantity { get; init; }

    /// <summary>The order returning the stock. Only what that order took is returned, once.</summary>
    [Required]
    public Guid OrderId { get; init; }
}