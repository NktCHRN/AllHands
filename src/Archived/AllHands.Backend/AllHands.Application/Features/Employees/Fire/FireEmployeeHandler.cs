using AllHands.Application.Abstractions;
using AllHands.Domain.Events.Employee;
using AllHands.Domain.Exceptions;
using AllHands.Domain.Models;
using Marten;
using MediatR;

namespace AllHands.Application.Features.Employees.Fire;

public sealed class FireEmployeeHandler(IDocumentSession documentSession, IAccountService accountService, ICurrentUserService currentUserService) : IRequestHandler<FireEmployeeCommand>
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

        if (employee.ManagerId != currentUserService.GetEmployeeId()
            && !currentUserService.IsAllowed(Application.Permissions.EmployeeEdit))
        {
            throw new ForbiddenForUserException("Only managers and users with permission can fire employees.");
        }
        
        await accountService.DeactivateAsync(employee.UserId, cancellationToken);

        documentSession.Events.Append(employee.Id, new EmployeeFiredEvent(
            employee.Id,
            currentUserService.GetId(),
            request.Reason));
        await documentSession.SaveChangesAsync(cancellationToken);
    }
}
