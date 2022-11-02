using MediatR;
using EcommerceDDD.Orders.Domain;
using EcommerceDDD.Orders.Domain.Events;
using EcommerceDDD.Orders.Domain.Commands;
using EcommerceDDD.Orders.Application.Shipments.ShippingPackage;
using EcommerceDDD.Orders.Application.Payments.CancelingPayment;
using EcommerceDDD.Orders.Application.Payments.RequestingPayment;

namespace EcommerceDDD.Orders.Application;

/// <summary>
/// Handles compensation events for OrderSaga
/// </summary>
public class OrderSagaCompensation :     
    INotificationHandler<OrderCanceled>,
    INotificationHandler<ProductWasOutOfStock>,
    INotificationHandler<CustomerReachedCreditLimit>
{
    private readonly IMediator _mediator;

    public OrderSagaCompensation(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    // Order canceled from Shipments
    public async Task Handle(ProductWasOutOfStock @event, CancellationToken cancellationToken)
    {
        var command = CancelOrder.Create(
            OrderId.Of(@event.OrderId), 
            OrderCancellationReason.ProductWasOutOfStock);

        await _mediator.Send(command, cancellationToken);
    }

    // Order canceled from Payments
    public async Task Handle(CustomerReachedCreditLimit @event, CancellationToken cancellationToken)
    {
        var command = CancelOrder.Create(
            OrderId.Of(@event.OrderId), 
            OrderCancellationReason.CustomerReachedCreditLimit);

        await _mediator.Send(command, cancellationToken);
    }

    // Requesting cancelling payment
    public async Task Handle(OrderCanceled @event, CancellationToken cancellationToken)
    {
        switch (@event.OrderCancellationReason)
        {
            case OrderCancellationReason.ProductWasOutOfStock:
                var command = RequestCancelPayment.Create(PaymentId.Of(@event.PaymentId!.Value), PaymentCancellationReason.OrderCanceled);
                await _mediator.Send(command, cancellationToken);
                break;
            default:
                break;
        }    
    }
}