namespace EcommerceDDD.Core.Infrastructure.Kafka.Serialization;

public class JsonEventSerializer<T> : ISerializer<T>, IDeserializer<T?>
    where T : class
{
    public byte[] Serialize(T data, SerializationContext context)
        => Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));

    public T? Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    {
        if (isNull)
            return null;

        var eventType = GetEventType(context);
        if (eventType == null)
            return null;

        var message = Encoding.UTF8.GetString(data);
        var @event = JsonConvert.DeserializeObject(message, eventType) as T;
        return @event;
    }

    public Type? GetEventType(SerializationContext context)
    {
        var eventTypeName = Encoding.UTF8.GetString(context.Headers.GetLastBytes("eventType"));
        string className = eventTypeName.Substring(eventTypeName.LastIndexOf('.') + 1);
        return TypeGetter.GetTypeFromCurrentDomainAssembly(className);
    }
}