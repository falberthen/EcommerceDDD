using System;
using System.Collections.Generic;
using System.Text;

namespace EcommerceDDD.Application.Orders.PlaceOrder
{
    public class PlaceOrderRequest
    {
        public List<ProductDto> Products { get; set; }
        public string Currency { get; set; }
    }
}
