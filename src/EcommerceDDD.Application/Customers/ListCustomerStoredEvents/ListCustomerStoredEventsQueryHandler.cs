using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EcommerceDDD.Domain;
using BuildingBlocks.CQRS.QueryHandling;
using EcommerceDDD.Application.EventSourcing.StoredEventsData;
using EcommerceDDD.Application.EventSourcing;

namespace EcommerceDDD.Application.Customers.ListCustomerStoredEvents
{
    public class ListCustomerStoredEventsQueryHandler : QueryHandler<ListCustomerStoredEventsQuery, 
        IList<CustomerStoredEventData>> 
    {
        private readonly IEcommerceUnitOfWork _unitOfWork;

        public ListCustomerStoredEventsQueryHandler(IEcommerceUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public override async Task<IList<CustomerStoredEventData>> ExecuteQuery(ListCustomerStoredEventsQuery request, CancellationToken cancellationToken)
        {
            var storedEvents = await _unitOfWork.StoredEvents
                .GetByAggregateId(request.CustomerId, cancellationToken);

            IList<CustomerStoredEventData> customerStoredEvents = StoredEventPrettier<CustomerStoredEventData>
                .ToPretty(storedEvents);

            return customerStoredEvents;
        }
    }
}
