using EcommerceDDD.Core.EventBus;
using EcommerceDDD.Orders.Domain;
using EcommerceDDD.Orders.Domain.Events;
using EcommerceDDD.Orders.Domain.Commands;
using EcommerceDDD.Core.CQRS.CommandHandling;
using EcommerceDDD.Orders.Application.Payments.CancelingPayment;
using EcommerceDDD.Orders.Application.Payments.RequestingPayment;
using EcommerceDDD.Orders.Application.Shipments.RequestingShipment;
using EcommerceDDD.Orders.Application.Payments.ProcessingPayment;

namespace EcommerceDDD.Orders.Application;

/// <summary>
/// Handles compensation events for OrderSaga
/// </summary>
public class OrderSagaCompensation :
    IEventHandler<PaymentFailed>,
    IEventHandler<CustomerReachedCreditLimit>,
    IEventHandler<ProductWasOutOfStock>,
    IEventHandler<OrderCanceled>
{
    private readonly ICommandBus _commandBus;

    public OrderSagaCompensation(ICommandBus commandBus)
    {
        _commandBus = commandBus;
    }

    public Task Handle(PaymentFailed @event, CancellationToken cancellationToken)
    {
        var command = CancelOrder.Create(
            OrderId.Of(@event.OrderId),
            OrderCancellationReason.PaymentFailed);

        return _commandBus.Send(command);
    }

    public Task Handle(CustomerReachedCreditLimit @event, CancellationToken cancellationToken)
    {
        var command = CancelOrder.Create(
            OrderId.Of(@event.OrderId),
            OrderCancellationReason.CustomerReachedCreditLimit);

        return _commandBus.Send(command);
    }

    public Task Handle(ProductWasOutOfStock @event, CancellationToken cancellationToken)
    {
        var command = CancelOrder.Create(
            OrderId.Of(@event.OrderId),
            OrderCancellationReason.ProductWasOutOfStock);

        return _commandBus.Send(command);
    }

    public Task Handle(OrderCanceled @event, CancellationToken cancellationToken)
    {
        if (@event.PaymentId.HasValue) // if order was paid but canceled
        {
            var command = RequestCancelPayment.Create(PaymentId.Of(@event.PaymentId!.Value), PaymentCancellationReason.OrderCanceled);
            return _commandBus.Send(command);
        }

        return Task.CompletedTask;
    }
}