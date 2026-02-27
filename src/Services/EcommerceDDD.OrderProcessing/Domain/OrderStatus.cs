namespace EcommerceDDD.OrderProcessing.Domain;

public enum OrderStatus
{
    Placed = 1,
    Processed = 2,
    Paid = 3,
    Shipped = 4,
    Delivered = 5,
    Canceled = 0
}