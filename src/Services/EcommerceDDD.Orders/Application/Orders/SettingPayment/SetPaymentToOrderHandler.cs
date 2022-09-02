using MediatR;
using EcommerceDDD.Core.Persistence;
using EcommerceDDD.Orders.Domain;
using EcommerceDDD.Core.CQRS.CommandHandling;
using EcommerceDDD.IntegrationServices.Orders;
using EcommerceDDD.IntegrationServices.Orders.Requests;
using EcommerceDDD.Orders.Application.Orders.RequestingShipment;
using EcommerceDDD.IntegrationServices;
using Microsoft.Extensions.Options;
using EcommerceDDD.Core.Infrastructure.Kafka.Consumer;

namespace EcommerceDDD.Orders.Application.Orders.SettingPayment;

public class SetPaymentToOrderHandler : CommandHandler<SetPaymentToOrder>
{
    private readonly IMediator _mediator;
    private readonly IServiceProvider _serviceProvider;
    private readonly IntegrationServicesSettings _integrationServicesSettings;

    public SetPaymentToOrderHandler(
        IMediator mediator,
        IServiceProvider serviceProvider,
        IOptions<IntegrationServicesSettings> integrationServicesSettings)
    {
        if (integrationServicesSettings == null)
            throw new ArgumentNullException(nameof(integrationServicesSettings));
        
        _mediator = mediator;
        _serviceProvider = serviceProvider;
        _integrationServicesSettings = integrationServicesSettings.Value;
    }

    public override async Task Handle(SetPaymentToOrder command, CancellationToken cancellationToken)
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

        // Recording the payment
        order.RecordPayment(command.PaymentId, command.TotalPaid);
        await orderWriteRepository
            .AppendEventsAsync(order);

        // Requesting shipment
        var requestShipment = new RequestShipment(order.Id, order.OrderLines);
        await _mediator.Send(requestShipment);

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