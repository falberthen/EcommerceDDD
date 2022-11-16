using MediatR;
using EcommerceDDD.Payments.Domain;
using EcommerceDDD.Core.Persistence;
using EcommerceDDD.Payments.Domain.Commands;
using EcommerceDDD.Core.CQRS.CommandHandling;

namespace EcommerceDDD.Payments.Application.RequestingPayment;

public class RequestPaymentHandler : ICommandHandler<RequestPayment>
{
    private readonly ICommandBus _commandBus;
    private readonly IEventStoreRepository<Payment> _paymentWriteRepository;

    public RequestPaymentHandler(
        ICommandBus commandBus,
        IEventStoreRepository<Payment> paymentWriteRepository)
    {
        _commandBus = commandBus;
        _paymentWriteRepository = paymentWriteRepository;
    }

    public async Task<Unit> Handle(RequestPayment command, CancellationToken cancellationToken)
    {
        var paymentData = new PaymentData(
            command.CustomerId, 
            command.OrderId, 
            command.TotalAmount);
 
        var payment = Payment.Create(paymentData);

        await _paymentWriteRepository
            .AppendEventsAsync(payment);

        await _commandBus.Send(ProcessPayment.Create(payment.Id));
        return Unit.Value;
    }
}