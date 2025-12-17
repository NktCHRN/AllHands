using Wolverine;
using Wolverine.AmazonSns;
using Wolverine.AmazonSqs;

namespace AllHands.Shared.Infrastructure.Messaging;

public static class WolverineOptionsExtensions
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

    private static string GetQueueName(string service, string topic) => $"{service}_{topic}";
    private static string GetDeadLetterQueueName(string queue) => $"{queue}_errors";
}
