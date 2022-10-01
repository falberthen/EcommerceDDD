using MediatR;
using EcommerceDDD.Core.Domain;
using EcommerceDDD.Orders.Domain;
using EcommerceDDD.Orders.Domain.Events;
using EcommerceDDD.Orders.Domain.Commands;
using EcommerceDDD.Orders.Application.Payments.FinalizingPayment;
using EcommerceDDD.Orders.Application.Shipments.FinalizingShipment;
using EcommerceDDD.Orders.Application.Payments.RequestingPayment;
using EcommerceDDD.Orders.Application.Shipments.RequestingShipment;

namespace EcommerceDDD.Orders.Application;

public class OrderSaga : 
    INotificationHandler<DomainNotification<OrderPlaced>>,
    INotificationHandler<DomainNotification<OrderPaid>>,
    INotificationHandler<PaymentFinalized>,
    INotificationHandler<OrderDelivered>
{
    private readonly IMediator _mediator;

    public OrderSaga(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Handle(DomainNotification<OrderPlaced> notification, CancellationToken cancellationToken)
    {
        var @event = notification.DomainEvent;
        var command = RequestPayment.Create(
            CustomerId.Of(@event.CustomerId),
            OrderId.Of(@event.OrderId),
            Money.Of(@event.TotalPrice, @event.CurrencyCode),
            Currency.OfCode(@event.CurrencyCode));

        await _mediator.Send(command, cancellationToken);
    }

    public async Task Handle(PaymentFinalized @event, CancellationToken cancellationToken)
    {
        await DelayOnPurpose(3000);

        var command = RecordPayment.Create(
            PaymentId.Of(@event.PaymentId),
            OrderId.Of(@event.OrderId),
            Money.Of(@event.TotalAmount, @event.CurrencyCode));

        await _mediator.Send(command, cancellationToken);
    }

    public async Task Handle(DomainNotification<OrderPaid> notification, CancellationToken cancellationToken)
    {
        await DelayOnPurpose(3000);

        var @event = notification.DomainEvent;
        var command = RequestShipment.Create(OrderId.Of(@event.OrderId));
        await _mediator.Send(command, cancellationToken);
    }

    public async Task Handle(OrderDelivered @event, CancellationToken cancellationToken)
    {
        await DelayOnPurpose(3000);

        var command = CompleteOrder.Create(
            OrderId.Of(@event.OrderId), 
            ShipmentId.Of(@event.ShipmentId));

        await _mediator.Send(command, cancellationToken);
    }

    // Delaying just to allow you to see the statuses changing on the UI =)
    private async Task DelayOnPurpose(int milliseconts)
    {
        await Task.Delay(milliseconts);
    }
}