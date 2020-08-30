using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EcommerceDDD.Application.Base.Queries;
using EcommerceDDD.Application.EventSourcing.EventHistoryData;
using EcommerceDDD.Application.EventSourcing;
using EcommerceDDD.Infrastructure.Messaging;
using EcommerceDDD.Domain;

namespace EcommerceDDD.Application.Customers.ListCustomerEventHistory
{
    public class ListCustomerEventsQueryHandler : IQueryHandler<ListCustomerEventHistoryQuery, IList<CustomerHistoryData>> 
    {
        private readonly IEcommerceUnitOfWork _unitOfWork;

        public ListCustomerEventsQueryHandler(
            IEcommerceUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IList<CustomerHistoryData>> Handle(ListCustomerEventHistoryQuery request, CancellationToken cancellationToken)
        {
            IList<CustomerHistoryData> categoryHistoryData = new List<CustomerHistoryData>();
            var storedEvents = await _unitOfWork.MessageRepository.GetByAggregateId(request.CustomerId, cancellationToken);
            
            CustomerEventNormatizer normatizer = new CustomerEventNormatizer();
            categoryHistoryData = normatizer.ToHistoryData(storedEvents);
            return categoryHistoryData;
        }
    }
}
