using AllHands.EmployeeService.Application.Abstractions;
using AllHands.EmployeeService.Domain.Models;
using AllHands.Shared.Contracts.Messaging.Events.Employees;
using AllHands.Shared.Domain.Exceptions;
using AllHands.Shared.Domain.UserContext;
using Marten;
using MediatR;
using EmployeeRehiredEvent = AllHands.EmployeeService.Domain.Events.Employee.EmployeeRehiredEvent;

namespace AllHands.EmployeeService.Application.Features.Employees.Rehire;

public sealed class RehireEmployeeHandler(IDocumentSession documentSession, IEventService eventService, IUserContext userContext) : IRequestHandler<RehireEmployeeCommand>
{
    public async Task Handle(RehireEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employee = await documentSession.Query<Employee>()
                           .FirstOrDefaultAsync(e => e.Id == request.EmployeeId, cancellationToken)
                       ?? throw new EntityNotFoundException("Employee was not found.");

        if (employee.Status != EmployeeStatus.Fired)
        {
            throw new EntityAlreadyExistsException("This employee is not fired, no need to rehire them.");
        }
        
        documentSession.Events.Append(employee.Id, new EmployeeRehiredEvent(request.EmployeeId, userContext.Id));
        await eventService.PublishAsync(new AllHands.Shared.Contracts.Messaging.Events.Employees.EmployeeRehiredEvent(request.EmployeeId, employee.CompanyId, employee.UserId));
        await eventService.PublishAsync(new EmployeeStatusUpdated(request.EmployeeId, employee.StatusBeforeFire.ToString(),
            employee.CompanyId, employee.UserId));
        await documentSession.SaveChangesAsync(cancellationToken);
    }
}
