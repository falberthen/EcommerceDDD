using System;
using System.Collections.Generic;
using System.Text;

namespace EcommerceDDD.Application.EventSourcing.StoredEvents
{
    public class CustomerStoredEventData : StoredEventData
    {
        public string Name { get; set; }
    }
}
