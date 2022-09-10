using MediatR;
using EcommerceDDD.Orders.Domain;
using EcommerceDDD.Orders.Domain.Events;
using EcommerceDDD.Orders.Application.Shipments.ShippingPackage;
using EcommerceDDD.Orders.Application.Orders.CancelingOrder;
using EcommerceDDD.Orders.Application.Payments.CancelingPayment;
using EcommerceDDD.Core.Domain;
using EcommerceDDD.Orders.Application.Payments.ProcessingPayment;

namespace EcommerceDDD.Orders.Application;

/// <summary>
/// Handles compensation events for OrderSaga
/// </summary>
public class OrderSagaCompensation :     
    INotificationHandler<DomainEventNotification<OrderCanceled>>,
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
        var orderId = OrderId.Of(@event.OrderId);
        var command = new CancelOrder(orderId, OrderCancellationReason.ProductWasOutOfStock);
        await _mediator.Send(command, cancellationToken);
    }

    // Order canceled from Payments
    public async Task Handle(CustomerReachedCreditLimit @event, CancellationToken cancellationToken)
    {
        var orderId = OrderId.Of(@event.OrderId);
        var command = new CancelOrder(orderId, OrderCancellationReason.CustomerReachedCreditLimit);
        await _mediator.Send(command, cancellationToken);
    }

    // Requesting cancelling payment
    public async Task Handle(DomainEventNotification<OrderCanceled> notification, CancellationToken cancellationToken)
    {
        var @event = notification.DomainEvent;
        var command = new RequestCancelPayment(@event.PaymentId, PaymentCancellationReason.OrderCanceled);
        await _mediator.Send(command);
    }
}