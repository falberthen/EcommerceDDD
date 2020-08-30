using System;
using System.Collections.Generic;
using System.Text;

namespace EcommerceDDD.Application.Orders.ChangeOrder
{
    public class ChangeOrderRequest
    {
        public List<ProductDto> Products { get; set; }
        public string Currency { get; set; }
    }
}
