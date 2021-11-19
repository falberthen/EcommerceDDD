using System.Threading;
using EcommerceDDD.Domain;
using System.Threading.Tasks;
using EcommerceDDD.Domain.Orders;
using System.Collections.Generic;
using EcommerceDDD.Application.Core.EventSourcing;
using EcommerceDDD.Domain.Payments.Specifications;
using EcommerceDDD.Application.Core.CQRS.QueryHandling;
using EcommerceDDD.Application.Core.EventSourcing.StoredEventsData;
using EcommerceDDD.Infrastructure.Events;

namespace EcommerceDDD.Application.Orders.ListOrderStoredEvents;

public class ListOrderStoredEventsQueryHandler : QueryHandler<ListOrderStoredEventsQuery, 
    IList<StoredEventData>>
{
    private readonly IEcommerceUnitOfWork _unitOfWork;
    private readonly IStoredEvents _storedEvents;

    public ListOrderStoredEventsQueryHandler(IEcommerceUnitOfWork unitOfWork, IStoredEvents storedEvents)
    {
        _unitOfWork = unitOfWork;
        _storedEvents = storedEvents;
    }

    public override async Task<IList<StoredEventData>> ExecuteQuery(ListOrderStoredEventsQuery request, 
        CancellationToken cancellationToken)
    {
        List<StoredEventData> storedEvents = new List<StoredEventData>();
            
        var orderStoredEvents = await _storedEvents
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
            var paymentStoredEvents = await _storedEvents
                .GetByAggregateId(payment[0].Id.Value, cancellationToken);

            storedEvents.AddRange(StoredEventPrettier<StoredEventData>
                .ToPretty(paymentStoredEvents));
        }
                        
        return storedEvents;
    }
}