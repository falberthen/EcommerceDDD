using System;

namespace EcommerceDDD.Application.Carts.CreateCart
{
    public record SaveCartRequest
    {
        public Guid CustomerId { get; set; }
        public ProductDto Product { get; set; }
        public string Currency { get; set; }
    }
}
