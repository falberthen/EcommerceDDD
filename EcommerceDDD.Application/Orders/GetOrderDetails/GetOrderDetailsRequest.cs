using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EcommerceDDD.Application.Orders.PlaceOrder
{
    public class GetOrderDetailsRequest
    {
        [Required(ErrorMessage = "The {0} field is required.")]
        public Guid OrderId { get; set; }

        [Required(ErrorMessage = "The {0} field is required.")]
        public string Currency { get; set; }
    }
}
