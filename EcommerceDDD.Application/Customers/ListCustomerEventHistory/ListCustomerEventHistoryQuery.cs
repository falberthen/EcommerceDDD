using System;
using System.Collections.Generic;
using System.Text;
using EcommerceDDD.Application.Base.Queries;
using EcommerceDDD.Application.EventSourcing.EventHistoryData;

namespace EcommerceDDD.Application.Customers.ListCustomerEventHistory
{
    public class ListCustomerEventHistoryQuery : IQuery<IList<CustomerHistoryData>>
    {
        public ListCustomerEventHistoryQuery(Guid customerId)
        {
            CustomerId = customerId;
        }

        public Guid CustomerId { get; }
    }
}
