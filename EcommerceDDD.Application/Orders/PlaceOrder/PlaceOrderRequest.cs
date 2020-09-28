using System;
using System.ComponentModel.DataAnnotations;

namespace EcommerceDDD.Application.Orders.PlaceOrder
{
    public class PlaceOrderRequest
    {
        [Required(ErrorMessage = "The {0} field is required.")]
        public Guid CustomerId { get; set; }

        [Required(ErrorMessage = "The {0} field is required.")]
        public string Currency { get; set; }
    }
}
