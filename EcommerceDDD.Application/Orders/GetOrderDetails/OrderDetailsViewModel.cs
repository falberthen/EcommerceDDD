using EcommerceDDD.Application.Customers.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EcommerceDDD.Application.Orders.GetOrderDetails
{
    public class OrderDetailsViewModel
    {
        public Guid OrderId { get; set; }
        public List<OrderLinesDetailsViewModel> OrderLines { get; set; } = new List<OrderLinesDetailsViewModel>();
        public double TotalPrice { get; private set; }
        public string CreatedAt { get; set; }

        public void CalculateTotalOrderPrice()
        {
            var sum = OrderLines.Sum(s => s.ProductPrice);
            TotalPrice = Convert.ToDouble(sum);
        }
    }

    public class OrderLinesDetailsViewModel
    {
        public Guid ProductId { get; set; }
        public String ProductName { get; set; }
        public decimal ProductPrice { get; set; }
        public int ProductQuantity { get; set; }
        public String CurrencySymbol { get; set; }
    }
}