using System.Text;
using Confluent.Kafka;
using Newtonsoft.Json;
using EcommerceDDD.Core.Reflection;
using EcommerceDDD.Core.EventBus;

namespace EcommerceDDD.Core.Infrastructure.Kafka.Events;

public static class KafkaEventExtensions
{
    public static IIntegrationEvent? ToIntegrationEvent(this ConsumeResult<string, string> message)
    {
        if (message.Message is null)
            return null;

        var eventTypeName = RemoveEncoding(message.Key);
        var eventType = TypeGetter.GetTypeFromCurrencDomainAssembly(eventTypeName);

        if (eventType is null)
            return null;

        var settings = new JsonSerializerSettings()
        {
            ContractResolver = new PrivateResolver(),
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
        };

        var @event = JsonConvert
            .DeserializeObject(RemoveEncoding(message.Message.Value), eventType, settings) as IIntegrationEvent;

        return @event;
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
