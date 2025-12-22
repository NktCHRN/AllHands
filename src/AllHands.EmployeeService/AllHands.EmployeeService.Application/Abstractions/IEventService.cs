using AllHands.Shared.Contracts.Messaging.Events;

namespace AllHands.EmployeeService.Application.Abstractions;

public interface IEventService
{
    Task PublishAsync<TEvent>(TEvent @event) where TEvent : IAllHandsEvent;
}
