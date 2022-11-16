using EcommerceDDD.Core.EventBus;
using EcommerceDDD.Orders.Domain;
using EcommerceDDD.Orders.Domain.Events;
using EcommerceDDD.Orders.Domain.Commands;
using EcommerceDDD.Core.CQRS.CommandHandling;
using EcommerceDDD.Orders.Application.Payments.RequestingPayment;
using EcommerceDDD.Orders.Application.Shipments.RequestingShipment;
using EcommerceDDD.Orders.Application.Shipments.ShippingPackage;
using EcommerceDDD.Orders.Application.Payments.ProcessingPayment;

namespace EcommerceDDD.Orders.Application;

public class OrderSaga :
    IEventHandler<OrderPlaced>,
    IEventHandler<OrderPaid>,
    IEventHandler<PaymentCompleted>,
    IEventHandler<PackageShipped>
{
    private readonly ICommandBus _commandBus;

    public OrderSaga(ICommandBus commandBus)
    {
        _commandBus = commandBus;
    }

    public Task Handle(OrderPlaced @event, CancellationToken cancellationToken)
    {
        var command = RequestPayment.Create(
            CustomerId.Of(@event.CustomerId),
            OrderId.Of(@event.OrderId),
            Money.Of(@event.TotalPrice, @event.CurrencyCode),
            Currency.OfCode(@event.CurrencyCode));

        return _commandBus.Send(command);
    }

    public Task Handle(PaymentCompleted @event, CancellationToken cancellationToken)
    {
        DelayOnPurpose(3000);

        var command = RecordPayment.Create(
            OrderId.Of(@event.OrderId),
            PaymentId.Of(@event.PaymentId),
            Money.Of(@event.TotalAmount, @event.CurrencyCode));

        return _commandBus.Send(command);
    }

    public Task Handle(OrderPaid @event, CancellationToken cancellationToken)
    {
        DelayOnPurpose(3000);

        var command = RequestShipment.Create(OrderId.Of(@event.OrderId));
        return _commandBus.Send(command);
    }

    public Task Handle(PackageShipped @event, CancellationToken cancellationToken)
    {
        DelayOnPurpose(3000);

        var command = CompleteOrder.Create(
            OrderId.Of(@event.OrderId),
            ShipmentId.Of(@event.ShipmentId));

        return _commandBus.Send(command);
    }

    // Delaying just to allow you to see the statuses changing on the UI =)
    private Task DelayOnPurpose(int milliseconts)
    {
        return Task.Delay(milliseconts);
    }
}