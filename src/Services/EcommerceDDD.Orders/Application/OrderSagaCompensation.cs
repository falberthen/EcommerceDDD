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
    IEventHandler<OrderCanceled>,
    IEventHandler<ProductWasOutOfStock>,
    IEventHandler<CustomerReachedCreditLimit>,
    IEventHandler<PaymentFailed>
{
    private readonly ICommandBus _commandBus;

    public OrderSagaCompensation(ICommandBus commandBus)
    {
        _commandBus = commandBus;
    }

    public async Task Handle(PaymentFailed @event, CancellationToken cancellationToken)
    {
        var command = CancelOrder.Create(
            OrderId.Of(@event.OrderId),
            OrderCancellationReason.PaymentFailed);

        await _commandBus.Send(command);
    }

    public async Task Handle(ProductWasOutOfStock @event, CancellationToken cancellationToken)
    {
        var command = CancelOrder.Create(
            OrderId.Of(@event.OrderId),
            OrderCancellationReason.ProductWasOutOfStock);

        await _commandBus.Send(command);
    }

    public async Task Handle(CustomerReachedCreditLimit @event, CancellationToken cancellationToken)
    {
        var command = CancelOrder.Create(
            OrderId.Of(@event.OrderId),
            OrderCancellationReason.CustomerReachedCreditLimit);

        await _commandBus.Send(command);
    }

    // Requesting cancelling payment
    public async Task Handle(OrderCanceled @event, CancellationToken cancellationToken)
    {
        ICommand command;
        switch (@event.OrderCancellationReason)
        {
            case OrderCancellationReason.ProductWasOutOfStock:
                command = RequestCancelPayment.Create(PaymentId.Of(@event.PaymentId!.Value), PaymentCancellationReason.OrderCanceled);
                await _commandBus.Send(command);
                break;
            case OrderCancellationReason.PaymentFailed:
                command = RequestCancelPayment.Create(PaymentId.Of(@event.PaymentId!.Value), PaymentCancellationReason.ProcessmentError);
                await _commandBus.Send(command);
                break;
            default:
                break;
        }
    }
}