using System.Text.Json;
using AllHands.NewsService.Application.Features.Employees.Save;
using AllHands.Shared.Contracts.Messaging.Events.Employees;
using AllHands.Shared.Domain.UserContext;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AllHands.NewsService.ConsumersWorker.Handlers;

public sealed class EmployeeCreatedHandler(ILogger<EmployeeCreatedHandler> logger, IMediator mediator, IUserContextAccessor userContextAccessor)
{
    public async Task Handle(EmployeeCreatedEvent @event, CancellationToken cancellationToken)
    {
        logger.LogInformation("Received EmployeeCreatedEvent {Event}", @event);
        logger.LogInformation(JsonSerializer.Serialize(userContextAccessor.UserContext));
        
        await mediator.Send(new SaveEmployeeCommand(
            @event.Id,
            @event.FirstName,
            @event.MiddleName,
            @event.LastName,
            @event.Email,
            @event.CompanyId,
            @event.OccurredAt), cancellationToken);
        
        logger.LogInformation("Processed EmployeeCreatedEvent {Event}", @event);
    }
}
