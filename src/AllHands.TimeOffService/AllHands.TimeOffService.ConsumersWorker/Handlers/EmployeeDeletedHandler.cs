using AllHands.Shared.Contracts.Messaging.Events.Employees;
using AllHands.TimeOffService.Application.Features.Employees.Delete;
using MediatR;

namespace AllHands.TimeOffService.ConsumersWorker.Handlers;

public sealed class EmployeeDeletedHandler(ILogger<EmployeeDeletedHandler> logger, IMediator mediator)
{
    public async Task Handle(EmployeeDeletedEvent @event, CancellationToken cancellationToken)
    {
        logger.LogInformation("Received EmployeeDeletedEvent {Event}", @event);
        
        await mediator.Send(new DeleteEmployeeCommand(@event.Id), cancellationToken);
        
        logger.LogInformation("Processed EmployeeDeletedEvent {Event}", @event);
    }
}
