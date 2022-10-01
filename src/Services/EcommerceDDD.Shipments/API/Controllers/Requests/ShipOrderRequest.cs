using System.ComponentModel.DataAnnotations;

namespace EcommerceDDD.Shipments.API.Controllers.Requests;

public record class ShipOrderRequest
{
    [Required(ErrorMessage = "The {0} field is required.")]
    public Guid OrderId { get; init; }

    [Required(ErrorMessage = "The {0} field is required.")]
    public IReadOnlyList<ProductItemRequest> ProductItems { get; init; }
}

public record ProductItemRequest(
    Guid ProductId, 
    string ProductName, 
    decimal UnitPrice, 
    int Quantity);