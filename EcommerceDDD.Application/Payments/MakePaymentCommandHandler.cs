using System;
using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.CQRS.CommandHandling;
using EcommerceDDD.Application.Base;
using EcommerceDDD.Domain;
using EcommerceDDD.Domain.Services;

namespace EcommerceDDD.Application.Payments
{
    public class MakePaymentCommandHandler : CommandHandler<MakePaymentCommand, Guid>
    {
        private readonly IEcommerceUnitOfWork _unitOfWork;
        private readonly IOrderStatusWorkflow _orderStatusWorkflowManager;

        public MakePaymentCommandHandler(IEcommerceUnitOfWork unitOfWork, 
            IOrderStatusWorkflow orderStatusManager)
        {
            _unitOfWork = unitOfWork;
            _orderStatusWorkflowManager = orderStatusManager;
        }

        public override async Task<Guid> ExecuteCommand(MakePaymentCommand command, CancellationToken cancellationToken)
        {            
            var payment = await _unitOfWork.PaymentRepository.GetById(command.PaymentId, cancellationToken);

            if (payment == null)
                throw new InvalidDataException("Payment not found.");

            if (payment.Order == null)
                throw new InvalidDataException("Order not found.");

            // Making a fake payment (here we could call a financial institution service and use the customer billing info)
            payment.MarkAsPaid();

            // Changing order status
            _orderStatusWorkflowManager.CalculateOrderStatus(payment.Order, payment);
            await _unitOfWork.CommitAsync();
            return payment.Id;
        }
    }
}
