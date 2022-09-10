namespace EcommerceDDD.Orders.Domain;

public enum OrderStatus
{
    Placed = 1,
    Paid = 2,
    Shipped = 3,
    Completed = 4,
    Canceled = 0
}