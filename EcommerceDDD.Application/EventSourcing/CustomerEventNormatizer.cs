using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EcommerceDDD.Application.EventSourcing.StoredEvents;
using EcommerceDDD.Domain.Core.Messaging;

namespace EcommerceDDD.Application.EventSourcing
{
    public class CustomerEventNormatizer : StoredEventNormatizer<CustomerStoredEventData>
    {
        public override IList<CustomerStoredEventData> ToHistoryData(IList<StoredEvent> messages)
        {            
            IList<CustomerStoredEventData> historyData = GetHistoryData(messages);
            var sortedEvent = historyData.OrderBy(c => c.Timestamp);
            var categoryHistory = new List<CustomerStoredEventData>();
            var last = new CustomerStoredEventData();

            foreach (var change in sortedEvent)
            {
                var data = new CustomerStoredEventData();
                data.Id = change.Id == Guid.Empty.ToString() || change.Id == last.Id ? string.Empty : change.Id;
                data.Name = string.IsNullOrWhiteSpace(change.Name) || change.Name == last.Name ? string.Empty : change.Name;
                data.Action = string.IsNullOrWhiteSpace(change.Action) ? string.Empty : change.Action;
                data.Timestamp = change.Timestamp;
                categoryHistory.Add(data);
                last = change;
            }

            return categoryHistory;            
        }

    }
}
