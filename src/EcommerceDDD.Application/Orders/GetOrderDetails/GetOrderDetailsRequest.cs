using System.ComponentModel.DataAnnotations;

namespace EcommerceDDD.Application.Orders.GetOrderDetails
{
    public record class GetOrderDetailsRequest
    {
        [Required(ErrorMessage = "The {0} field is required.")]
        public Guid OrderId { get; init; }

        [Required(ErrorMessage = "The {0} field is required.")]
        public string Currency { get; init; }
    }
}
