using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using EcommerceDDD.Application.EventSourcing.StoredEvents;
using EcommerceDDD.Domain.Core.Messaging;
using Newtonsoft.Json;

namespace EcommerceDDD.Application.EventSourcing
{
    public abstract class StoredEventNormatizer<THistoryData> where THistoryData : StoredEventData, new()
    {
        public abstract IList<THistoryData> ToHistoryData(IList<StoredEvent> messages);

        protected IList<THistoryData> GetHistoryData(IList<StoredEvent> messages)
        {
            IList<THistoryData> historyData = new List<THistoryData>();

            foreach (var message in messages)
            {
                var eventName = message.MessageType.Substring(message.MessageType.LastIndexOf('.')).Substring(1);
                THistoryData history = JsonConvert.DeserializeObject<THistoryData>(message.Payload);
                history.Id = message.Id.ToString();
                history.Timestamp = message.CreatedAt.ToString("yyyy'-'MM'-'dd' - 'HH':'mm':'ss");                 
                history.Action = eventName;
                historyData.Add(history);
            }

            return historyData;
        }
    }
}
