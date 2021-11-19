namespace EcommerceDDD.Application.Core.EventSourcing.StoredEventsData;

public record CustomerStoredEventData : StoredEventData
{
    public string Name { get; set; }
}

