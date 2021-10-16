namespace EcommerceDDD.Application.Core.EventSourcing.StoredEventsData
{
    /// <summary>
    /// Common fields to display with the stored event payload data
    /// </summary>
    public class StoredEventData
    {
        public string Id { get; set; }
        public string Action { get; set; }
        public string Timestamp { get; set; }
    }
}
