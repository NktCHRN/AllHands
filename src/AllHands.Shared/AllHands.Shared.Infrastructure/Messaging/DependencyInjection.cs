using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Wolverine;
using Wolverine.AmazonSns;
using Wolverine.AmazonSqs;
using Wolverine.Runtime;

namespace AllHands.Shared.Infrastructure.Messaging;

public static class DependencyInjection
{
    extension(WolverineOptions options)
    {
        public void AddListener<TEvent>(string environment, string topic, string service)
        {
            ArgumentNullException.ThrowIfNull(options);
        
            var queue = GetQueueName(environment, service, topic);
        
            options.ListenToSqsQueue(queue)
                .ConfigureDeadLetterQueue(GetDeadLetterQueueName(queue));
    
            options.PublishMessage<TEvent>()
                .ToSnsTopic(GetTopicName(environment, topic))
                .SubscribeSqsQueue(queue, sub => sub.RawMessageDelivery = true);
        }

        public void AddPublisher<TEvent>(string environment, string topic)
        {
            ArgumentNullException.ThrowIfNull(options);
        
            options.PublishMessage<TEvent>()
                .ToSnsTopic(GetTopicName(environment, topic));
        }
    }
    
    public static IHostApplicationBuilder UseAllHandsWolverine(this IHostApplicationBuilder builder, Action<WolverineOptions>? configure)
    {
        return builder.UseWolverine(options =>
        {
            options.UseAmazonSqsTransport().AutoProvision();
    
            options.UseAmazonSnsTransport().AutoProvision();
            
            configure?.Invoke(options);
        });
    }

    public static IServiceCollection AddContextAwareBus(this IServiceCollection services)
    {
        services.TryAddScoped<Domain.UserContext.UserContext>();
        
        services.AddScoped<MessageContext>();
        services.AddScoped<IMessageBus>(sp => new ContextAwareBus(sp.GetRequiredService<MessageContext>(), sp.GetRequiredService<Domain.UserContext.UserContext>()));
        
        return services;
    }

    private static string GetQueueName(string environment, string service, string topic) => $"{environment.ToLower()}_{service.ToLower()}_{topic}";
    private static string GetTopicName(string environment, string topic) => $"{environment.ToLower()}_{topic}";
    private static string GetDeadLetterQueueName(string queue) => $"{queue}_errors";
}
