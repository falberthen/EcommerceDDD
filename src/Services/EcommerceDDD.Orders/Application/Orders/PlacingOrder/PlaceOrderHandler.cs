using MediatR;
using EcommerceDDD.Core.Persistence;
using EcommerceDDD.Orders.Domain;
using EcommerceDDD.Orders.Domain.Events;
using EcommerceDDD.Core.CQRS.CommandHandling;

namespace EcommerceDDD.Orders.Application.Orders.PlacingOrder;

public class PlaceOrderHandler : CommandHandler<PlaceOrder>
{
    private readonly IEventStoreRepository<Order> _orderWriteRepository;
    private readonly IOrderProductsChecker _orderProductsChecker;
    private readonly IMediator _mediator;

    public PlaceOrderHandler(
        IEventStoreRepository<Order> orderWriteRepository,
        IOrderProductsChecker orderProductsChecker,
        IMediator mediator)
    {
        _orderWriteRepository = orderWriteRepository;
        _orderProductsChecker = orderProductsChecker;
        _mediator = mediator;
    }

    public override async Task Handle(PlaceOrder command, CancellationToken cancellationToken)
    {
        var order = await Order
            .CreateNew(command.OrderId, command.ConfirmedQuote, _orderProductsChecker);

        var @event = order.GetUncommittedEvents()
            .FirstOrDefault(e => e.GetType() == typeof(OrderPlaced));

        await _orderWriteRepository
            .AppendEventsAsync(order);

        // This call cannot be awaited, due to the fact the Saga workflow has to happen asynchronously
        _mediator.Publish(@event, cancellationToken);
    }
}