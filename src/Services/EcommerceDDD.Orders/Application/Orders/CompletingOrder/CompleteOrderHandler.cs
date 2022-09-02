using EcommerceDDD.Core.Persistence;
using EcommerceDDD.Orders.Domain;
using EcommerceDDD.Core.CQRS.CommandHandling;
using EcommerceDDD.IntegrationServices.Orders;
using EcommerceDDD.IntegrationServices.Orders.Requests;
using EcommerceDDD.IntegrationServices;
using Microsoft.Extensions.Options;

namespace EcommerceDDD.Orders.Application.Orders.CompletingOrder;

public class CompleteOrderHandler : CommandHandler<CompleteOrder>
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IntegrationServicesSettings _integrationServicesSettings;

    public CompleteOrderHandler(
        IServiceProvider serviceProvider,
        IOptions<IntegrationServicesSettings> integrationServicesSettings)
    {
        if (integrationServicesSettings == null)
            throw new ArgumentNullException(nameof(integrationServicesSettings));

        _serviceProvider = serviceProvider;
        _integrationServicesSettings = integrationServicesSettings.Value;
    }

    public override async Task Handle(CompleteOrder command, CancellationToken cancellationToken)
    {
        if (_serviceProvider == null)
            throw new ArgumentNullException(nameof(_serviceProvider));

        using var scopedService = _serviceProvider.CreateScope();
        var orderWriteRepository = scopedService
            .ServiceProvider.GetRequiredService<IEventStoreRepository<Order>>();
        var ordersService = scopedService
           .ServiceProvider.GetRequiredService<IOrdersService>();
        
        var order = await orderWriteRepository
            .FetchStream(command.OrderId.Value);

        if (order == null)
            throw new ApplicationException($"Failed to find the order {command.OrderId}.");

        // Completing order
        order.Complete();
        await orderWriteRepository
            .AppendEventsAsync(order);
        
        // Updating order status on the UI with SignalR
        await ordersService.UpdateOrderStatus(
            _integrationServicesSettings.ApiGatewayBaseUrl,
            new UpdateOrderStatusRequest(
                order.CustomerId.Value,
                command.OrderId.Value,
                order.Status.ToString(),
                (int)order.Status));
    }
}