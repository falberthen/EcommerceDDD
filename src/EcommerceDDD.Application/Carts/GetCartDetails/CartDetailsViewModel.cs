using System;
using System.Collections.Generic;
using System.Linq;

namespace EcommerceDDD.Application.Carts.GetCartDetails
{
    public class CartDetailsViewModel
    {
        public Guid CartId { get; set; }
        public List<CartItemDetailsViewModel> CartItems { get; set; } = new List<CartItemDetailsViewModel>();
        public double TotalPrice { get; private set; }
        public string CreatedAt { get; set; }

        public void CalculateTotalOrderPrice()
        {
            var sum = CartItems.Sum(s => s.ProductPrice * s.ProductQuantity);
            TotalPrice = Convert.ToDouble(sum);
        }
    }

    public class CartItemDetailsViewModel
    {
        public Guid ProductId { get; set; }
        public String ProductName { get; set; }
        public decimal ProductPrice { get; set; }
        public int ProductQuantity { get; set; }
        public String CurrencySymbol { get; set; }
    }
}