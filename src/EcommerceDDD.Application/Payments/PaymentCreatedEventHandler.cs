using MediatR;
using System.Threading;
using System.Threading.Tasks;
using EcommerceDDD.Domain.Payments.Events;
using EcommerceDDD.Domain;
using EcommerceDDD.Application.Orders;
using EcommerceDDD.Domain.Orders;
using EcommerceDDD.Application.Core.ExceptionHandling;

namespace EcommerceDDD.Application.Payments
{
    public class PaymentCreatedEventHandler : INotificationHandler<PaymentCreatedEvent>
    {
        private readonly IEcommerceUnitOfWork _unitOfWork;
        private readonly IOrderStatusWorkflow _orderStatusWorkflow;
        private readonly IMediator _mediator;
        private readonly IOrderStatusBroadcaster _orderStatusBroadcaster;

        public PaymentCreatedEventHandler(
            IEcommerceUnitOfWork unitOfWork,
            IOrderStatusWorkflow orderStatusWorkflow,            
            IMediator mediator,
            IOrderStatusBroadcaster orderStatusBroadcaster)
        {
            _unitOfWork = unitOfWork;
            _orderStatusWorkflow = orderStatusWorkflow;
            _mediator = mediator;
            _orderStatusBroadcaster = orderStatusBroadcaster;
        }

        public async Task Handle(PaymentCreatedEvent paymentCreatedEvent
            , CancellationToken cancellationToken)
        {
            var payment = await _unitOfWork.Payments
                .GetById(paymentCreatedEvent.PaymentId, cancellationToken);

            var customer = await _unitOfWork.Customers
                .GetById(payment.CustomerId, cancellationToken);

            var order = await _unitOfWork.Orders
                .GetById(payment.OrderId, cancellationToken);

            if (payment == null)
                throw new ApplicationDataException("Payment not found.");

            if (customer == null)
                throw new ApplicationDataException("Customer not found.");

            if (order == null)
                throw new ApplicationDataException("order not found.");

            // Changing order status
            _orderStatusWorkflow.CalculateOrderStatus(order, payment);
            await _unitOfWork.CommitAsync(cancellationToken);

            // Attempting to pay
            MakePaymentCommand command = new MakePaymentCommand(paymentCreatedEvent.PaymentId.Value);
            await _mediator.Send(command, cancellationToken);

            // Broadcasting order update
            await _orderStatusBroadcaster.BroadcastOrderStatus(
                customer.Id,
                order.Id,
                order.Status
            );
        }
    }
}
