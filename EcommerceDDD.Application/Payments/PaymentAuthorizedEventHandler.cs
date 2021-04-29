using MediatR;
using System.Threading;
using System.Threading.Tasks;
using EcommerceDDD.Domain.Payments.Events;
using System.IO;
using EcommerceDDD.Domain;
using EcommerceDDD.Domain.Services;

namespace EcommerceDDD.Application.Orders.PlaceOrder
{
    public class PaymentAuthorizedEventHandler : INotificationHandler<PaymentAuthorizedEvent>
    {
        private readonly IEcommerceUnitOfWork _unitOfWork;
        private readonly IOrderStatusWorkflow _orderStatusWorkflow;

        public PaymentAuthorizedEventHandler(
            IEcommerceUnitOfWork unitOfWork,
            IOrderStatusWorkflow orderStatusWorkflow)
        {
            _unitOfWork = unitOfWork;
            _orderStatusWorkflow = orderStatusWorkflow;
        }

        public async Task Handle(PaymentAuthorizedEvent paymentAuthorizedEvent, CancellationToken cancellationToken)
        {
            var payment = await _unitOfWork.PaymentRepository.GetById(paymentAuthorizedEvent.PaymentId, cancellationToken);

            if (payment == null)
                throw new InvalidDataException("Payment not found.");

            if (payment.Order == null)
                throw new InvalidDataException("Order not found.");

            // Changing order status
            _orderStatusWorkflow.CalculateOrderStatus(payment.Order, payment);
            await _unitOfWork.CommitAsync(cancellationToken);
        }
    }
}
