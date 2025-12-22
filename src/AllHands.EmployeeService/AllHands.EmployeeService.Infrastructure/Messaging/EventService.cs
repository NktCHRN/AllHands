using AllHands.EmployeeService.Application.Abstractions;
using AllHands.Shared.Contracts.Messaging.Events;
using AllHands.Shared.Domain.UserContext;
using AllHands.Shared.Infrastructure.Messaging;
using Wolverine.Marten;

namespace AllHands.EmployeeService.Infrastructure.Messaging;

public class EventService(IMartenOutbox messageBus, IUserContextAccessor userContextAccessor) : IEventService
{
    public async Task PublishAsync<TEvent>(TEvent @event) where TEvent : IAllHandsEvent
    {
        await messageBus.PublishWithHeadersAsync(@event, userContextAccessor.UserContext ?? throw new InvalidOperationException("UserContext is not set."));
    }
}
