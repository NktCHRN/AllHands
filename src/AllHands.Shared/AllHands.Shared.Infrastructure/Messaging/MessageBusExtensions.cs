using System.Text.Json;
using AllHands.Shared.Contracts.Messaging.Events;
using AllHands.Shared.Infrastructure.UserContext;
using AllHands.Shared.Infrastructure.Utilities;
using Wolverine;

namespace AllHands.Shared.Infrastructure.Messaging;

public static class MessageBusExtensions
{
    public static ValueTask PublishWithHeadersAsync<T>(this IMessageBus messageBus, T message, Domain.UserContext.IUserContext userContext, DeliveryOptions? options = null) where T : IAllHandsEvent
    {
        options ??= new DeliveryOptions();

        options.GroupId = message.GroupId;

        if (userContext is null)
        {
            throw new InvalidOperationException("Setup UserContext to use this method");
        }
        
        options.Headers.Remove(UserContextHeaders.Id);
        options.Headers.Remove(UserContextHeaders.CompanyId);
        options.Headers.Remove(UserContextHeaders.EmployeeId);
        options.Headers.Remove(UserContextHeaders.Email);
        options.Headers.Remove(UserContextHeaders.FirstName);
        options.Headers.Remove(UserContextHeaders.LastName);
        options.Headers.Remove(UserContextHeaders.PhoneNumber);
        options.Headers.Remove(UserContextHeaders.MiddleName);
        options.Headers.Remove(UserContextHeaders.Permissions);
        options.Headers.Remove(UserContextHeaders.Roles);
        
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
}
