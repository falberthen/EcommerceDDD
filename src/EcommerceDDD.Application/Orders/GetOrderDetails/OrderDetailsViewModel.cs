using System.Linq;
using System.Collections.Generic;

namespace EcommerceDDD.Application.Orders.GetOrderDetails
{
    public record class OrderDetailsViewModel
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

    public record class OrderLinesDetailsViewModel
    {
        public Guid ProductId { get; init; }
        public string ProductName { get; init; }
        public decimal ProductPrice { get; init; }
        public int ProductQuantity { get; init; }
        public string CurrencySymbol { get; init; }
    }

    public record class OrderStatusViewModel
    {
        public int StatusCode { get; init; }
        public string StatusText { get; init; }

        public OrderStatusViewModel(int statusCode, string statusText)
        {
            StatusCode = statusCode;
            StatusText = statusText;
        }
    }
}