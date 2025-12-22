using AllHands.Application.Abstractions;
using AllHands.Domain.Events.Employee;
using AllHands.Domain.Exceptions;
using AllHands.Domain.Models;
using Marten;
using MediatR;

namespace AllHands.Application.Features.Employees.Rehire;

public sealed class RehireEmployeeHandler(IDocumentSession documentSession, ICurrentUserService currentUserService, IAccountService accountService) : IRequestHandler<RehireEmployeeCommand>
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
        
        await accountService.ReactivateAsync(employee.UserId, cancellationToken);

        documentSession.Events.Append(employee.Id, new EmployeeRehiredEvent(request.EmployeeId, currentUserService.GetId()));
        await documentSession.SaveChangesAsync(cancellationToken);
    }
}