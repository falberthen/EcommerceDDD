using EcommerceDDD.Application.Orders.GetOrderDetails;
using EcommerceDDD.Domain.Orders;

namespace EcommerceDDD.Application.Orders
{
    public static class OrderStatusPrettier
    {
        public static OrderStatusViewModel Prettify(OrderStatus status)
        {
            return status switch
            {
                OrderStatus.Placed => 
                    new OrderStatusViewModel((int)OrderStatus.Placed, "Order placed."),
                OrderStatus.WaitingForPayment => 
                    new OrderStatusViewModel((int)OrderStatus.WaitingForPayment, "Waiting for payment to be processed..."),
                OrderStatus.ReadyToShip => 
                    new OrderStatusViewModel((int)OrderStatus.ReadyToShip, "Ready to be shipped."),
                _ => new OrderStatusViewModel(0, string.Empty)
            };
        }
    }
}