using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EcommerceDDD.Application.EventSourcing;
using EcommerceDDD.Domain;
using EcommerceDDD.Application.EventSourcing.StoredEvents;
using BuildingBlocks.CQRS.QueryHandling;

namespace EcommerceDDD.Application.Customers.ListCustomerStoredEvents
{
    public class ListCustomerStoredEventsQueryHandler : QueryHandler<ListCustomerStoredEventsQuery, IList<CustomerStoredEventData>> 
    {
        private readonly IEcommerceUnitOfWork _unitOfWork;

        public ListCustomerStoredEventsQueryHandler(
            IEcommerceUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public override async Task<IList<CustomerStoredEventData>> ExecuteQuery(ListCustomerStoredEventsQuery request, CancellationToken cancellationToken)
        {
            IList<CustomerStoredEventData> customerStoredEvents = new List<CustomerStoredEventData>();
            var storedEvents = await _unitOfWork.MessageRepository.GetByAggregateId(request.CustomerId, cancellationToken);
            
            CustomerEventNormatizer normatizer = new CustomerEventNormatizer();
            customerStoredEvents = normatizer.ToHistoryData(storedEvents);
            return customerStoredEvents;
        }
    }
}
