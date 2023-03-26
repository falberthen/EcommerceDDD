using MediatR;
using EcommerceDDD.Core.Persistence;
using EcommerceDDD.Orders.Domain;
using EcommerceDDD.Core.CQRS.CommandHandling;
using EcommerceDDD.Orders.Domain.Commands;
using EcommerceDDD.Core.Exceptions;

namespace EcommerceDDD.Orders.Application.Orders.RecordingPayment;

public class RecordPaymentHandler : ICommandHandler<RecordPayment>
{
    private readonly IEventStoreRepository<Order> _orderWriteRepository;

    public RecordPaymentHandler(IEventStoreRepository<Order> orderWriteRepository)
    {
        _orderWriteRepository = orderWriteRepository;
    }

    public async Task Handle(RecordPayment command, CancellationToken cancellationToken)
    {
        var order = await _orderWriteRepository
            .FetchStreamAsync(command.OrderId.Value)
            ?? throw new RecordNotFoundException($"Failed to find the order {command.OrderId}.");

        // Recording the payment
        order.RecordPayment(command.PaymentId, command.TotalPaid);
        await _orderWriteRepository
            .AppendEventsAsync(order);
    }
}