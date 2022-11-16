using EcommerceDDD.Core.EventBus;
using EcommerceDDD.Core.Infrastructure.Kafka.Consumer;
using EcommerceDDD.Core.Infrastructure.Kafka.Producer;
using EcommerceDDD.Core.Infrastructure.Kafka.Serialization;
using EcommerceDDD.Core.Infrastructure.Workers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace EcommerceDDD.Core.Infrastructure.Kafka;

public static class KafkaSetup
{
    public static IServiceCollection AddKafkaProducer(this IServiceCollection services, IConfiguration configuration)
    {
        var producerConfig = configuration.GetSection("KafkaProducer");
        services.Configure<KafkaProducerConfig>(producerConfig);
        services.TryAddSingleton<IEventProducer, KafkaProducer>();
        return services;
    }

    public static IServiceCollection AddKafkaConsumer(this IServiceCollection services, IConfiguration configuration)
    {
        var consumerConfig = configuration.GetSection("KafkaConsumer");
        services.Configure<KafkaConsumerConfig>(consumerConfig);
        services.AddSingleton(typeof(JsonEventSerializer<>));
        services.TryAddSingleton<IEventConsumer, KafkaConsumer>();

        return services.AddHostedService(serviceProvider =>
        {
            var consumer = serviceProvider.GetRequiredService<IEventConsumer>();
            return new BackgroundWorker(consumer.StartConsumeAsync);
        });
    }
}