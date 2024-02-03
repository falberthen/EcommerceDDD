using EcommerceDDD.Core.Reflection;

namespace EcommerceDDD.Core.Infrastructure.Kafka.Serialization;

public class JsonEventSerializer<T> : ISerializer<T>, IDeserializer<T>
    where T : class
{
    public byte[] Serialize(T data, SerializationContext context)
        => Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));

    public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    {
        if (isNull)
            return default!;

        var jsonString = Encoding.UTF8.GetString(data.ToArray());
        var jsonObject = JObject.Parse(jsonString);

        var eventName = jsonObject["EventName"]?.ToString();
        if (string.IsNullOrEmpty(eventName))
            throw new JsonSerializationException("EventName is missing in the JSON payload.");

        var eventType = GetEventType(eventName);
        if (eventType == null)
            throw new JsonSerializationException($"Cannot find type for event name: {eventName}");

        var eventData = jsonObject["JSON_Payload"]?.ToString();
        var @event = JsonConvert.DeserializeObject(eventData, eventType) as T;
        return @event!;
    }

    public Type? GetEventType(string eventTypeName)
    {
        return TypeGetter.GetTypeFromCurrentDomainAssembly(eventTypeName);
    }
}