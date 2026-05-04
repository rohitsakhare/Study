using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Messaging.Abstractions;
using Shared.Messaging.Kafka;

namespace Shared.Messaging.Extensions;

public static class MessagingExtensions
{
    public static IServiceCollection AddKafkaProducer(this IServiceCollection services)
    {
        services.AddSingleton<IEventBus, EventBus>();
        return services;
    }

    public static IServiceCollection AddKafkaConsumer<TConsumer, TMessage>(this IServiceCollection services, IConfiguration config, string topic)
        where TConsumer : class, IEventConsumer<TMessage>
    {
        services.AddScoped<IEventConsumer<TMessage>, TConsumer>();

        services.AddHostedService(sp => new EventConsumer<TMessage>(config, sp.GetRequiredService<IServiceScopeFactory>(), topic));

        return services;
    }
}