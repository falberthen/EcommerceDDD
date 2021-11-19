namespace EcommerceDDD.Application.Core.EventSourcing.StoredEventsData;

public record class CustomerStoredEventData : StoredEventData
{
    public string Name { get; init; }
}