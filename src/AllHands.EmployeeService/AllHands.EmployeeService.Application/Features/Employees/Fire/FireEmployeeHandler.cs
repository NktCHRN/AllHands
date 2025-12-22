using AllHands.EmployeeService.Application.Abstractions;
using AllHands.EmployeeService.Domain.Models;
using AllHands.Shared.Application.Auth;
using AllHands.Shared.Contracts.Messaging.Events.Employees;
using AllHands.Shared.Domain.Exceptions;
using AllHands.Shared.Domain.UserContext;
using Marten;
using MediatR;
using EmployeeFiredEvent = AllHands.EmployeeService.Domain.Events.Employee.EmployeeFiredEvent;

namespace AllHands.EmployeeService.Application.Features.Employees.Fire;

public sealed class FireEmployeeHandler(IDocumentSession documentSession, IEventService eventService, IUserContext userContext, IUserPermissionService userPermissionService) : IRequestHandler<FireEmployeeCommand>
{
    public async Task Handle(FireEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employee = await documentSession.Query<Employee>()
                           .FirstOrDefaultAsync(e => e.Id == request.EmployeeId, cancellationToken)
                       ?? throw new EntityNotFoundException("Employee was not found.");

        if (employee.Status != EmployeeStatus.Active && employee.Status != EmployeeStatus.Unactivated)
        {
            throw new EntityAlreadyExistsException("Cannot update fired employee.");
        }

        if (employee.ManagerId != userContext.EmployeeId
            && !userPermissionService.IsAllowed(Permissions.EmployeeEdit))
        {
            throw new ForbiddenForUserException("Only managers and users with permission can fire employees.");
        }
        
        documentSession.Events.Append(employee.Id, new EmployeeFiredEvent(
            employee.Id,
            userContext.Id,
            request.Reason));
        await eventService.PublishAsync(new AllHands.Shared.Contracts.Messaging.Events.Employees.EmployeeFiredEvent(request.EmployeeId, employee.CompanyId, employee.UserId));
        await eventService.PublishAsync(new EmployeeStatusUpdated(request.EmployeeId, nameof(EmployeeStatus.Fired),
            employee.CompanyId, employee.UserId));
        await documentSession.SaveChangesAsync(cancellationToken);
    }
}
