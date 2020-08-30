using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceDDD.Domain.Core.Messaging
{
    public class StoredEvent : Event
    {
        public StoredEvent(Event @event, string payload)
        {
            Id = Guid.NewGuid();
            AggregateId = @event.AggregateId;
            MessageType = @event.MessageType;
            Payload = payload;
        }

        public void SetProcessedAt(DateTime date)
        {
            if (date == null)
                throw new ArgumentNullException(nameof(date));
            ProcessedAt = date;
        }

        // EF Constructor
        protected StoredEvent() { }

        public Guid Id { get; private set; }
        public string Payload { get; private set; }
        public DateTime? ProcessedAt { get; private set; }       
    }
}
