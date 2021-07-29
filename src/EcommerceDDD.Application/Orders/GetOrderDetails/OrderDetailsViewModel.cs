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
        public OrderStatusViewModel Status { get; set; }

        public void CalculateTotalOrderPrice()
        {
            var sum = OrderLines.Sum(s => s.ProductPrice);
            TotalPrice = Convert.ToDouble(sum);
        }
    }

    public class OrderLinesDetailsViewModel
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal ProductPrice { get; set; }
        public int ProductQuantity { get; set; }
        public string CurrencySymbol { get; set; }
    }

    public class OrderStatusViewModel
    {
        public OrderStatusViewModel(int statusCode, string statusText)
        {
            StatusCode = statusCode;
            StatusText = statusText;
        }

        public int StatusCode { get; set; }
        public string StatusText { get; set; }
    }
}