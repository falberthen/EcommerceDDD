using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using EcommerceDDD.Application.Core.CQRS.QueryHandling;
using EcommerceDDD.Application.Core.EventSourcing;
using EcommerceDDD.Application.Core.EventSourcing.StoredEventsData;
using EcommerceDDD.Infrastructure.Events;

namespace EcommerceDDD.Application.Customers.ListCustomerStoredEvents;

public class ListCustomerStoredEventsQueryHandler : QueryHandler<ListCustomerStoredEventsQuery, 
    IList<CustomerStoredEventData>> 
{
    private readonly IStoredEvents _storedEvents;

    public ListCustomerStoredEventsQueryHandler(IStoredEvents storedEvents)
    {
        _storedEvents = storedEvents;
    }

    public override async Task<IList<CustomerStoredEventData>> ExecuteQuery(ListCustomerStoredEventsQuery request, 
        CancellationToken cancellationToken)
    {
        var storedEvents = await _storedEvents
            .GetByAggregateId(request.CustomerId, cancellationToken);

        IList<CustomerStoredEventData> customerStoredEvents = StoredEventPrettier<CustomerStoredEventData>
            .ToPretty(storedEvents);

        return customerStoredEvents;
    }
}