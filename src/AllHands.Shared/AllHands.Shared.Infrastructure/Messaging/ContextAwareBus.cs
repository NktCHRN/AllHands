using System.Text.Json;
using AllHands.Shared.Contracts.Messaging.Events;
using AllHands.Shared.Infrastructure.UserContext;
using AllHands.Shared.Infrastructure.Utilities;
using Wolverine;

namespace AllHands.Shared.Infrastructure.Messaging;

public class ContextAwareBus(IMessageBus messageBus, Domain.UserContext.UserContext userContext) : IMessageBus
{
    public Task InvokeAsync(object message, CancellationToken cancellation = new CancellationToken(), TimeSpan? timeout = null)
    {
        return messageBus.InvokeAsync(message, cancellation, timeout);
    }

    public Task InvokeAsync(object message, DeliveryOptions options, CancellationToken cancellation = new CancellationToken(),
        TimeSpan? timeout = null)
    {
        return messageBus.InvokeAsync(message, options, cancellation, timeout);
    }

    public Task<T> InvokeAsync<T>(object message, CancellationToken cancellation = new CancellationToken(), TimeSpan? timeout = null)
    {
        return messageBus.InvokeAsync<T>(message, cancellation, timeout);
    }

    public Task<T> InvokeAsync<T>(object message, DeliveryOptions options, CancellationToken cancellation = new CancellationToken(),
        TimeSpan? timeout = null)
    {
        return messageBus.InvokeAsync<T>(message, options, cancellation, timeout);
    }

    public Task InvokeForTenantAsync(string tenantId, object message, CancellationToken cancellation = new CancellationToken(),
        TimeSpan? timeout = null)
    {
        return messageBus.InvokeForTenantAsync(tenantId, message, cancellation, timeout);
    }

    public Task<T> InvokeForTenantAsync<T>(string tenantId, object message, CancellationToken cancellation = new CancellationToken(),
        TimeSpan? timeout = null)
    {
        return messageBus.InvokeForTenantAsync<T>(tenantId, message, cancellation, timeout);
    }

    public IDestinationEndpoint EndpointFor(string endpointName)
    {
        return messageBus.EndpointFor(endpointName);
    }

    public IDestinationEndpoint EndpointFor(Uri uri)
    {
        return messageBus.EndpointFor(uri);
    }

    public IReadOnlyList<Envelope> PreviewSubscriptions(object message)
    {
        return messageBus.PreviewSubscriptions(message);
    }

    public IReadOnlyList<Envelope> PreviewSubscriptions(object message, DeliveryOptions options)
    {
        return messageBus.PreviewSubscriptions(message, options);
    }

    public ValueTask SendAsync<T>(T message, DeliveryOptions? options = null)
    {
        return messageBus.SendAsync<T>(message, options);
    }

    public ValueTask PublishAsync<T>(T message, DeliveryOptions? options = null)
    {
        options ??= new DeliveryOptions();

        if (message is IAllHandsEvent @event)
        {
            options.GroupId = @event.GroupId;
        }
        
        options.Headers.Add(UserContextHeaders.Id, userContext.Id.ToString());
        options.Headers.Add(UserContextHeaders.CompanyId, userContext.CompanyId.ToString());
        options.Headers.Add(UserContextHeaders.EmployeeId, userContext.EmployeeId.ToString());
        options.Headers.Add(UserContextHeaders.Email, userContext.Email);
        options.Headers.Add(UserContextHeaders.FirstName, userContext.FirstName);
        options.Headers.Add(UserContextHeaders.LastName, userContext.LastName);
        options.Headers.Add(UserContextHeaders.Permissions, Convert.ToBase64String(userContext.Permissions.ToByteArray()));

        if (!string.IsNullOrEmpty(userContext.PhoneNumber))
        {
            options.Headers.Add(UserContextHeaders.PhoneNumber, userContext.PhoneNumber);
        }

        if (!string.IsNullOrEmpty(userContext.MiddleName))
        {
            options.Headers.Add(UserContextHeaders.MiddleName, userContext.MiddleName);
        }

        options.Headers.Add(UserContextHeaders.Roles, JsonSerializer.Serialize(userContext.Roles));
        
        options.TenantId = userContext.CompanyId.ToString();
        
        return messageBus.PublishAsync(message, options);
    }

    public ValueTask BroadcastToTopicAsync(string topicName, object message, DeliveryOptions? options = null)
    {
        return messageBus.BroadcastToTopicAsync(topicName, message, options);
    }

    public string? TenantId
    {
        get => messageBus.TenantId;
        set => messageBus.TenantId = value;
    }
}
