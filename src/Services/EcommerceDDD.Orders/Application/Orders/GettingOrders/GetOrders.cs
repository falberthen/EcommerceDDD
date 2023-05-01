namespace EcommerceDDD.Orders.Application.Orders.GettingOrders;

public record class GetOrders : IQuery<List<OrderViewModel>>
{
    public CustomerId CustomerId { get; private set; }

    public static GetOrders Create(CustomerId customerId)
    {
        if (customerId is null)
            throw new ArgumentNullException(nameof(customerId));

        return new GetOrders(customerId);
    }

    private GetOrders(CustomerId customerId)
    {
        CustomerId = customerId;
    }
}