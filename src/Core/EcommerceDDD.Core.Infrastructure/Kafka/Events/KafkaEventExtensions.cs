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

        var eventType = TypeGetter.GetTypeFromCurrencDomainAssembly(message.Message.Key);

        if (eventType is null)
            return null;

        var settings = new JsonSerializerSettings()
        {
            ContractResolver = new PrivateResolver(),
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
        };

        var @event = JsonConvert
            .DeserializeObject(message.Message.Value, eventType, settings) as IIntegrationEvent;

        return @event;
    }
}
