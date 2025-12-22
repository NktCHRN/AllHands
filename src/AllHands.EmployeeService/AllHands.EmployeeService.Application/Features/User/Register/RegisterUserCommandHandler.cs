using AllHands.EmployeeService.Application.Abstractions;
using AllHands.EmployeeService.Domain.Models;
using AllHands.Shared.Contracts.Messaging.Events.Employees;
using AllHands.Shared.Domain.Exceptions;
using Marten;
using MediatR;
using EmployeeRegisteredEvent = AllHands.EmployeeService.Domain.Events.Employee.EmployeeRegisteredEvent;

namespace AllHands.EmployeeService.Application.Features.User.Register;

public sealed class RegisterUserCommandHandler(IDocumentSession documentSession, IEventService eventService) : IRequestHandler<RegisterUserCommand>
{
    public async Task Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var employee = await documentSession.Query<Employee>()
                           .FirstOrDefaultAsync(x => x.UserId == request.UserId, token: cancellationToken)
                       ?? throw new EntityNotFoundException("Employee was not found");
        
        documentSession.Events.Append(employee.Id, new EmployeeRegisteredEvent(employee.Id, employee.UserId));
        await eventService.PublishAsync(new AllHands.Shared.Contracts.Messaging.Events.Employees.EmployeeRegisteredEvent(employee.Id, employee.CompanyId, employee.UserId));
        await eventService.PublishAsync(new EmployeeStatusUpdated(employee.Id, nameof(EmployeeStatus.Active),
            employee.CompanyId, employee.UserId));

        await documentSession.SaveChangesAsync(cancellationToken);
    }
}
