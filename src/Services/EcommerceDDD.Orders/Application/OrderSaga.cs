using MediatR;
using EcommerceDDD.Orders.Domain;
using EcommerceDDD.Orders.Domain.Events;
using EcommerceDDD.Orders.Application.Orders.RequestingPayment;
using EcommerceDDD.Orders.Application.Orders.SettingPayment;
using EcommerceDDD.Orders.Application.Payments.FinalizingPayment;
using EcommerceDDD.Orders.Application.Orders.CompletingOrder;
using EcommerceDDD.Orders.Application.Shipments.FinalizingShipment;

namespace EcommerceDDD.Orders.Application;

public class OrderSaga : 
    INotificationHandler<OrderPlaced>,
    INotificationHandler<PaymentFinalized>,
    INotificationHandler<OrderDelivered>
{
    private readonly IMediator _mediator;

    public OrderSaga(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Handle(OrderPlaced @event, CancellationToken cancellationToken)
    {
        var command = new RequestPayment(
            @event.CustomerId, 
            @event.OrderId, 
            @event.TotalPrice, 
            @event.Currency.Code);

        await _mediator.Send(command);
    }

    public async Task Handle(PaymentFinalized @event, CancellationToken cancellationToken)
    {
        await DelayOnPurpose(3000);

        var paymentId = PaymentId.Of(@event.PaymentId);
        var orderId = OrderId.Of(@event.OrderId);
        var totalPaid = Money.Of(@event.TotalAmount, @event.CurrencyCode);
        var command = new SetPaymentToOrder(paymentId, orderId, totalPaid);

        await _mediator.Send(command);
    }

    public async Task Handle(OrderDelivered @event, CancellationToken cancellationToken)
    {
        await DelayOnPurpose(3000);

        var orderId = OrderId.Of(@event.OrderId);
        var command = new CompleteOrder(orderId);
        await _mediator.Send(command);
    }

    // Delaying just to allow you to see the statuses changing on the UI =)
    private async Task DelayOnPurpose(int milliseconts)
    {
        await Task.Delay(milliseconts);
    }
}