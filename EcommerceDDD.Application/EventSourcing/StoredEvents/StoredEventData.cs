using System;
using System.Collections.Generic;
using System.Text;

namespace EcommerceDDD.Application.EventSourcing.StoredEvents
{
    public class StoredEventData
    {
        public string Id { get; set; }
        public string Action { get; set; }
        public string Timestamp { get; set; }
    }
}
