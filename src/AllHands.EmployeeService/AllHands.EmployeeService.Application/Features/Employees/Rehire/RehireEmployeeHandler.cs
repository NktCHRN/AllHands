using AllHands.EmployeeService.Domain.Events.Employee;
using AllHands.EmployeeService.Domain.Models;
using AllHands.Shared.Domain.Exceptions;
using AllHands.Shared.Domain.UserContext;
using Marten;
using MediatR;

namespace AllHands.EmployeeService.Application.Features.Employees.Rehire;

public sealed class RehireEmployeeHandler(IDocumentSession documentSession, IUserContext userContext) : IRequestHandler<RehireEmployeeCommand>
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
        
        // TODO: send event.

        documentSession.Events.Append(employee.Id, new EmployeeRehiredEvent(request.EmployeeId, userContext.Id));
        await documentSession.SaveChangesAsync(cancellationToken);
    }
}
