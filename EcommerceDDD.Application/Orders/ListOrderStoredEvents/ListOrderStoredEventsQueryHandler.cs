using BuildingBlocks.CQRS.QueryHandling;
using EcommerceDDD.Application.EventSourcing;
using EcommerceDDD.Application.EventSourcing.StoredEventsData;
using EcommerceDDD.Domain;
using EcommerceDDD.Domain.Customers.Orders;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EcommerceDDD.Application.Orders.ListOrderStoredEvents
{
    public class ListOrderStoredEventsQueryHandler : QueryHandler<ListOrderStoredEventsQuery, IList<StoredEventData>>
    {
        private readonly IEcommerceUnitOfWork _unitOfWork;

        public ListOrderStoredEventsQueryHandler(IEcommerceUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public override async Task<IList<StoredEventData>> ExecuteQuery(ListOrderStoredEventsQuery request, CancellationToken cancellationToken)
        {
            List<StoredEventData> storedEvents = new List<StoredEventData>();
            
            var orderStoredEvents = await _unitOfWork.MessageRepository.GetByAggregateId(request.OrderId, cancellationToken);
            storedEvents.AddRange(StoredEventPrettier<StoredEventData>.ToPretty(orderStoredEvents));

            var orderId = OrderId.Of(request.OrderId);
            var payment = await _unitOfWork.PaymentRepository.GetPaymentByOrderId(orderId, cancellationToken);

            if(payment != null)
            {
                var paymentStoredEvents = await _unitOfWork.MessageRepository.GetByAggregateId(payment.Id.Value, cancellationToken);
                storedEvents.AddRange(StoredEventPrettier<StoredEventData>.ToPretty(paymentStoredEvents));
            }
                        
            return storedEvents;
        }
    }
}
