using EcommerceDDD.Application.Core.CQRS.QueryHandling;
using EcommerceDDD.Application.Core.EventSourcing;
using EcommerceDDD.Application.Core.EventSourcing.StoredEventsData;
using EcommerceDDD.Domain;
using EcommerceDDD.Domain.Orders;
using EcommerceDDD.Domain.Payments.Specifications;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EcommerceDDD.Application.Orders.ListOrderStoredEvents
{
    public class ListOrderStoredEventsQueryHandler : QueryHandler<ListOrderStoredEventsQuery, 
        IList<StoredEventData>>
    {
        private readonly IEcommerceUnitOfWork _unitOfWork;

        public ListOrderStoredEventsQueryHandler(IEcommerceUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public override async Task<IList<StoredEventData>> ExecuteQuery(ListOrderStoredEventsQuery request, 
            CancellationToken cancellationToken)
        {
            List<StoredEventData> storedEvents = new List<StoredEventData>();
            
            var orderStoredEvents = await _unitOfWork.StoredEvents
                .GetByAggregateId(request.OrderId, cancellationToken);

            storedEvents.AddRange(
                StoredEventPrettier<StoredEventData>
                .ToPretty(orderStoredEvents)
            );

            var orderId = OrderId.Of(request.OrderId);
            var isPaymentOfOrder = new IsPaymentOfOrder(orderId);
            var payment = await _unitOfWork.Payments
                .Find(isPaymentOfOrder, cancellationToken);

            if(payment.Count > 0)
            {
                var paymentStoredEvents = await _unitOfWork.StoredEvents
                    .GetByAggregateId(payment[0].Id.Value, cancellationToken);

                storedEvents.AddRange(StoredEventPrettier<StoredEventData>
                    .ToPretty(paymentStoredEvents));
            }
                        
            return storedEvents;
        }
    }
}
