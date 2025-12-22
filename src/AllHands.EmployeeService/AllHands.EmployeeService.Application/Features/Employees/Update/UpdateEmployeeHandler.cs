using AllHands.EmployeeService.Application.Abstractions;
using AllHands.EmployeeService.Domain.Models;
using AllHands.Shared.Contracts.Messaging.Events.Employees;
using AllHands.Shared.Domain.Exceptions;
using AllHands.Shared.Domain.UserContext;
using AllHands.Shared.Domain.Utilities;
using Marten;
using MediatR;
using Microsoft.Extensions.Logging;
using EmployeeUpdatedEvent = AllHands.EmployeeService.Domain.Events.Employee.EmployeeUpdatedEvent;

namespace AllHands.EmployeeService.Application.Features.Employees.Update;

public sealed class UpdateEmployeeHandler(IDocumentSession documentSession, IEventService eventService, IUserContext userContext, IUserClient userClient, ILogger<UpdateEmployeeHandler> logger) : IRequestHandler<UpdateEmployeeCommand>
{
    public async Task Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employee = await documentSession.Query<Employee>()
                           .FirstOrDefaultAsync(e => e.Id == request.EmployeeId, cancellationToken)
                       ?? throw new EntityNotFoundException("Employee was not found.");

        if (employee.Status != EmployeeStatus.Active && employee.Status != EmployeeStatus.Unactivated)
        {
            throw new EntityAlreadyExistsException("Cannot update fired employee.");
        }

        if (employee.PositionId != request.PositionId)
        {
            if (!await documentSession.Query<Position>().AnyAsync(e => e.Id == request.PositionId, cancellationToken))
            {
                throw new EntityNotFoundException("Position was not found.");
            }
        }

        if (employee.ManagerId != request.ManagerId)
        {
            if (!await documentSession.Query<Employee>().AnyAsync(e => e.Id == request.ManagerId, cancellationToken))
            {
                throw new EntityNotFoundException("Manager was not found.");
            }
        }

        await userClient.UpdateAsync(new UpdateIdentityUserCommand(
            employee.UserId,
            request.Email,
            request.FirstName,
            request.MiddleName,
            request.LastName,
            request.PhoneNumber,
            request.RoleId), cancellationToken);

        try
        {
            documentSession.Events.Append(employee.Id, new EmployeeUpdatedEvent(
                employee.Id,
                userContext.Id,
                request.PositionId,
                request.ManagerId,
                request.Email,
                StringUtilities.GetNormalizedEmail(request.Email),
                request.FirstName,
                request.MiddleName,
                request.LastName,
                request.PhoneNumber,
                request.WorkStartDate));
            await eventService.PublishAsync(new Shared.Contracts.Messaging.Events.Employees.EmployeeUpdatedEvent(
                employee.Id,
                request.FirstName,
                request.MiddleName,
                request.LastName,
                request.Email,
                request.PhoneNumber,
                request.WorkStartDate,
                request.ManagerId,
                request.PositionId,
                userContext.CompanyId,
                employee.UserId));
            await documentSession.SaveChangesAsync(cancellationToken);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while updating employee.");
            await userClient.UpdateAsync(new UpdateIdentityUserCommand(
                employee.UserId,
                employee.Email,
                employee.FirstName,
                employee.MiddleName,
                employee.LastName,
                employee.PhoneNumber,
                employee.RoleId), cancellationToken);
            
            throw;
        }
    }
}
