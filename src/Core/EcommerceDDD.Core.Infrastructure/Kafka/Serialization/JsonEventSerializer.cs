using System.Text;
using Newtonsoft.Json;
using Confluent.Kafka;
using EcommerceDDD.Core.Reflection;

namespace EcommerceDDD.Core.Infrastructure.Kafka.Serialization;

public class JsonEventSerializer<T> : ISerializer<T>, IDeserializer<T> where T : class
{
    public byte[] Serialize(T data, SerializationContext context)
        => Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));

    public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    {
        if (isNull)
            return null!;

        // TODO: try to add only classname as key of kafka topic
        // temporary solution:
        var eventTypeName = Encoding.UTF8.GetString(context.Headers.GetLastBytes("eventType"));
        string className = eventTypeName.Substring(eventTypeName.LastIndexOf('.') + 1);
        //
        
        var eventType = TypeGetter.GetTypeFromCurrencDomainAssembly(className);
        if (eventType is null)
            return null!;

        var settings = new JsonSerializerSettings()
        {
            ContractResolver = new PrivateResolver(),
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
        };

        var sanitizedData = RemoveEncoding(Encoding.UTF8.GetString(data));
        var @event = JsonConvert
            .DeserializeObject(sanitizedData, eventType, settings) as INotification;

        return (T)@event!;
    }

    private static string RemoveEncoding(string encodedJson)
    {
        var sb = new StringBuilder(encodedJson.Trim('"'));
        sb.Replace("\\", string.Empty);
        sb.Replace("\"[", "[");
        sb.Replace("]\"", "]");
        return sb.ToString();
    }
}