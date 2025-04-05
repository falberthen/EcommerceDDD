namespace EcommerceDDD.OrderProcessing.Application.Orders.GettingOrders;

public record class GetOrders : IQuery<IList<OrderViewModel>>
{
    public static GetOrders Create()
    {
        return new GetOrders();
    }

    private GetOrders() {}
}