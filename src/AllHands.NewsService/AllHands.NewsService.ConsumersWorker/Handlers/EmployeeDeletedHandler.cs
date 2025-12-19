using AllHands.NewsService.Application.Features.Employees.Delete;
using AllHands.Shared.Contracts.Messaging.Events.Employees;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AllHands.NewsService.ConsumersWorker.Handlers;

public sealed class EmployeeDeletedHandler(ILogger<EmployeeDeletedHandler> logger, IMediator mediator)
{
    public async Task Handle(EmployeeDeletedEvent @event, CancellationToken cancellationToken)
    {
        logger.LogInformation("Received EmployeeDeletedEvent {Event}", @event);
        
        await mediator.Send(new DeleteEmployeeCommand(
            @event.Id,
            @event.OccurredAt), cancellationToken);
        
        logger.LogInformation("Processed EmployeeDeletedEvent {Event}", @event);
    }
}
