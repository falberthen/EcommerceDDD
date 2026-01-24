namespace EcommerceDDD.OrderProcessing.Application.Orders.GettingOrders;

public record class GetOrders : IQuery<IReadOnlyList<OrderViewModel>>
{
    public static GetOrders Create()
    {
        return new GetOrders();
    }

    private GetOrders() {}
}