using System;
using System.Threading;
using System.Threading.Tasks;
using BuildingBlocks.CQRS.CommandHandling;
using EcommerceDDD.Application.Base;
using EcommerceDDD.Domain;
using EcommerceDDD.Domain.Payments;

namespace EcommerceDDD.Application.Payments
{
    public class MakePaymentCommandHandler : CommandHandler<MakePaymentCommand, Guid>
    {
        private readonly IEcommerceUnitOfWork _unitOfWork;

        public MakePaymentCommandHandler(
            IEcommerceUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public override async Task<Guid> ExecuteCommand(MakePaymentCommand command, 
            CancellationToken cancellationToken)
        {
            var paymentId = PaymentId.Of(command.PaymentId);
            var payment = await _unitOfWork.Payments
                .GetById(paymentId, cancellationToken);

            if (payment == null)
                throw new InvalidDataException("Payment not found.");

            if (payment.OrderId.Value == Guid.Empty)
                throw new InvalidDataException("Order not found.");
            
            try
            {
                // Making a fake payment
                // (here we could call a financial institution service and use the customer billing info)
                var paymentSuccess = MakePayment(payment);

                if (paymentSuccess)
                    payment.MarkAsPaid();
            }
            catch (Exception)
            {                
                throw;
            }

            return payment.Id.Value;
        }

        /// <summary>
        /// A simple mock that could be replaced by a real payment API call
        /// </summary>
        /// <param name="payment"></param>
        /// <param name="failRandomly">if true, it may simulate fail / not authorized</param>
        /// <returns></returns>
        private bool MakePayment(Payment payment, bool failRandomly = false)
        {
            bool paymentResult = true;            

            if (failRandomly)
            {
                var randomResult = new Random().Next(100) % 2 == 0;
                paymentResult = !randomResult 
                    ? throw new Exception($"Payment {payment.Id} not authorized.") 
                    : randomResult;
            }
                            
            return paymentResult;
        }
    }
}
