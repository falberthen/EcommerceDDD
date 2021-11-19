namespace EcommerceDDD.Domain.Orders;

public enum OrderStatus
{
    Placed = 1,
    WaitingForPayment = 2,
    ReadyToShip = 3,
    Canceled = 0
}