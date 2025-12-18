using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Wolverine;
using Wolverine.AmazonSns;
using Wolverine.AmazonSqs;
using Wolverine.Runtime;

namespace AllHands.Shared.Infrastructure.Messaging;

public static class DependencyInjection
{
    extension(WolverineOptions options)
    {
        public void AddListener<TEvent>(string topic, string service)
        {
            ArgumentNullException.ThrowIfNull(options);
        
            var queue = GetQueueName(service, topic);
        
            options.ListenToSqsQueue(queue)
                .ConfigureDeadLetterQueue(GetDeadLetterQueueName(queue));
    
            options.PublishMessage<TEvent>()
                .ToSnsTopic(topic)
                .SubscribeSqsQueue(queue, sub => sub.RawMessageDelivery = true);
        }

        public void AddPublisher<TEvent>(string topic)
        {
            ArgumentNullException.ThrowIfNull(options);
        
            options.PublishMessage<TEvent>()
                .ToSnsTopic(topic);
        }
    }

    public static IServiceCollection AddContextAwareBus(this IServiceCollection services)
    {
        services.TryAddScoped<Domain.UserContext.UserContext>();
        
        services.AddScoped<MessageContext>();
        services.AddScoped<IMessageBus>(sp => new ContextAwareBus(sp.GetRequiredService<MessageContext>(), sp.GetRequiredService<Domain.UserContext.UserContext>()));
        
        return services;
    }

    private static string GetQueueName(string service, string topic) => $"{service}_{topic}";
    private static string GetDeadLetterQueueName(string queue) => $"{queue}_errors";
}
