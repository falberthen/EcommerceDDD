using Newtonsoft.Json;

namespace EcommerceDDD.Core.Infrastructure.Outbox;

public class DebeziumConnectorConfig
{
    [JsonProperty("database.hostname")]
    public string DatabaseHostname { get; set; } // config

    [JsonProperty("database.port")] 
    public string DatabasePort { get; set; } // config

    [JsonProperty("database.user")] 
    public string DatabaseUser { get; set; } // config

    [JsonProperty("database.password")]
    public string DatabasePassword { get; set; } // config

    [JsonProperty("database.dbname")]
    public string DatabaseName { get; set; } // config

    [JsonProperty("database.server.name")]
    public string DatabaseServerName { get; set; } // config

    [JsonProperty("topic.prefix")]
    public string TopicPrefix { get; set; } // config

    [JsonProperty("slot.name")]
    public string SlotName { get; set; } // config

    [JsonProperty("transforms.outbox.route.topic.replacement")]
    public string TransformsOutboxRouteTopicReplacement { get; set; } // config

    [JsonProperty("connector.class")]
    public string ConnectorClass { get; } = "io.debezium.connector.postgresql.PostgresConnector";

    [JsonProperty("tasks.max")]
    public string TasksMax { get; } = "1";

    [JsonProperty("schema.include.list")]
    public string SchemaIncludeList { get; } = "public";

    [JsonProperty("table.include.list")]
    public string TableIncludeList { get; } = "public.OutboxMessages";

    [JsonProperty("tombstones.on.delete")]
    public string TombstonesOnDelete { get; } = "false";

    [JsonProperty("transforms")]
    public string transforms { get; } = "outbox";

    [JsonProperty("transforms.outbox.type")]
    public string TransformsOutboxType { get; } = "io.debezium.transforms.outbox.EventRouter";

    [JsonProperty("transforms.outbox.table.field.event.id")]
    public string TransformsOutboxTableFieldEventId { get; } = "Id";

    [JsonProperty("transforms.outbox.table.field.event.key")]
    public string TransformsOutboxTableFieldEventKey { get; } = "AggregateType";

    [JsonProperty("transforms.outbox.table.field.event.timestamp")]
    public string TransformsOutboxTableFieldEventTimestamp { get; } = "Timestamp";

    [JsonProperty("transforms.outbox.table.field.event.payload")]
    public string TransformsOutboxTableFieldEventPayload { get; } = "Payload";

    [JsonProperty("transforms.outbox.route.by.field")]
    public string TransformsOutboxRouteByField { get; } = "AggregateType";

    [JsonProperty("transforms.outbox.table.fields.additional.placement")] 
    public string TransformsOutboxTableFieldsAdditionalPlacement { get; } = "Type:header:eventType";

    [JsonProperty("transforms.outbox.debezium.expand.json.payload")]
    public string TransformsOutboxDebeziumExpandJsonPayload { get; } = "true";

    [JsonProperty("key.converter")]
    public string KeyConverter { get; } = "org.apache.kafka.connect.json.JsonConverter";

    [JsonProperty("key.converter.schemas.enable")]
    public string KeyConverterSchemasEnable { get; } = "false";

    [JsonProperty("value.converter")]
    public string ValueConverter { get; } = "org.apache.kafka.connect.json.JsonConverter";

    [JsonProperty("value.converter.schemas.enable")]
    public string ValueConverterSchemasEnable { get; } = "false";

    [JsonProperty("plugin.name")]
    public string PluginName { get; } = "pgoutput";
}