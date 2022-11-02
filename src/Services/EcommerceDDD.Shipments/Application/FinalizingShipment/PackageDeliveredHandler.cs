using MediatR;
using EcommerceDDD.Shipments.Domain.Events;
using EcommerceDDD.Core.Infrastructure.Outbox.Services;

namespace EcommerceDDD.Shipments.Application.FinalizingShipment;

public class PackageDeliveredHandler : INotificationHandler<PackageDelivered>
{
    private readonly IOutboxMessageService _outboxMessageService;

    public PackageDeliveredHandler(IOutboxMessageService outboxMessageService)
    {
        _outboxMessageService = outboxMessageService;
    }

    public async Task Handle(PackageDelivered @event, CancellationToken cancellationToken)
    {
        await _outboxMessageService.SaveAsOutboxMessageAsync(
            new OrderDelivered(
                @event.ShipmentId,
                @event.OrderId)
        );
    }
}