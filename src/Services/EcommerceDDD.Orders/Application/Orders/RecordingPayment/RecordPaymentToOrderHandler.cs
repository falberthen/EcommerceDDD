using MediatR;
using EcommerceDDD.Core.Persistence;
using EcommerceDDD.Orders.Domain;
using EcommerceDDD.Core.CQRS.CommandHandling;
using EcommerceDDD.IntegrationServices.Orders;
using EcommerceDDD.Orders.Application.Shipments.RequestingShipment;

namespace EcommerceDDD.Orders.Application.Orders.RecordingPayment;

public class RecordPaymentToOrderHandler : CommandHandler<RecordPaymentToOrder>
{
    private readonly IMediator _mediator;
    private readonly IServiceProvider _serviceProvider;

    public RecordPaymentToOrderHandler(
        IMediator mediator,
        IServiceProvider serviceProvider)
    {
        _mediator = mediator;
        _serviceProvider = serviceProvider;
    }

    public override async Task Handle(RecordPaymentToOrder command, CancellationToken cancellationToken)
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
    }
}