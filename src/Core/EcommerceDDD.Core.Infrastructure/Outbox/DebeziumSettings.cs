namespace EcommerceDDD.Core.Infrastructure.Outbox;

public record DebeziumSettings
{
    public string ConnectorUrl { get; set; }
    public string DatabaseHostname { get; set; }
    public string DatabaseServerName { get; set; }
    public string DatabasePort { get; set; }
    public string DatabaseUser { get; set; }
    public string DatabasePassword { get; set; }
    public string DatabaseName { get; set; }
    public string TopicPrefix { get; set; }
    public string TransformsTopicReplacement { get; set; }
    public string SlotName { get; set; }
    public string SchemaIncludeList { get; set; }
    public string TableIncludeList { get; set; }
}