using MediatR;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using EcommerceDDD.Domain.Payments.Events;
using EcommerceDDD.Domain.Services;
using EcommerceDDD.Domain;

namespace EcommerceDDD.Application.Payments
{
    public class PaymentCreatedEventHandler : INotificationHandler<PaymentCreatedEvent>
    {
        private readonly IEcommerceUnitOfWork _unitOfWork;
        private readonly IOrderStatusWorkflow _orderStatusWorkflow;
        private readonly IMediator _mediator;        

        public PaymentCreatedEventHandler(
            IEcommerceUnitOfWork unitOfWork,
            IOrderStatusWorkflow orderStatusWorkflow,            
            IMediator mediator)
        {
            _unitOfWork = unitOfWork;
            _orderStatusWorkflow = orderStatusWorkflow;
            _mediator = mediator;            
        }

        public async Task Handle(PaymentCreatedEvent paymentCreatedEvent, CancellationToken cancellationToken)
        {
            var payment = await _unitOfWork.PaymentRepository.GetById(paymentCreatedEvent.PaymentId, cancellationToken);

            if (payment == null)
                throw new InvalidDataException("Payment not found.");

            if (payment.Order == null)
                throw new InvalidDataException("Order not found.");

            // Changing order status
            _orderStatusWorkflow.CalculateOrderStatus(payment.Order, payment);
            await _unitOfWork.CommitAsync(cancellationToken);

            // Attempting to pay
            MakePaymentCommand command = new MakePaymentCommand(paymentCreatedEvent.PaymentId);
            await _mediator.Send(command, cancellationToken);          
        }
    }
}
