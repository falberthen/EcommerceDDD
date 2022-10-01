using MediatR;
using EcommerceDDD.Orders.Domain;
using EcommerceDDD.Core.Persistence;
using EcommerceDDD.Core.CQRS.CommandHandling;
using EcommerceDDD.Orders.Domain.Commands;

namespace EcommerceDDD.Orders.Application.Orders.PlacingOrder;

public class PlaceOrderHandler : ICommandHandler<PlaceOrder>
{
    private readonly IEventStoreRepository<Order> _orderWriteRepository;
    private readonly IProductItemsChecker _orderValidityChecker;

    public PlaceOrderHandler(
        IEventStoreRepository<Order> orderWriteRepository,
        IProductItemsChecker orderValidityChecker)
    {
        _orderWriteRepository = orderWriteRepository;
        _orderValidityChecker = orderValidityChecker;
    }

    public async Task<Unit> Handle(PlaceOrder command, CancellationToken cancellationToken)
    {
        await _orderValidityChecker
            .EnsureProductItemsExist(command.OrderData.Items, command.OrderData.Currency);

        var order = Order.Create(command.OrderData);

        await _orderWriteRepository
            .AppendEventsAsync(order);

        return Unit.Value;
    }
}