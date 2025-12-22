using AllHands.Application.Abstractions;
using AllHands.Domain.Events.Employee;
using AllHands.Domain.Exceptions;
using AllHands.Domain.Models;
using Marten;
using MediatR;

namespace AllHands.Application.Features.Employees.Delete;

public sealed class DeleteEmployeeHandler(IDocumentSession documentSession, IAccountService accountService, ICurrentUserService currentUserService) : IRequestHandler<DeleteEmployeeCommand>
{
    public async Task Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employee = await documentSession.Query<Employee>()
                           .FirstOrDefaultAsync(e => e.Id == request.EmployeeId, cancellationToken)
                       ?? throw new EntityNotFoundException("Employee was not found.");
        
        await accountService.DeleteAsync(employee.UserId, cancellationToken);
        
        documentSession.Events.Append(request.EmployeeId, new EmployeeDeletedEvent(request.EmployeeId, currentUserService.GetId(), request.Reason));
        await documentSession.SaveChangesAsync(cancellationToken);
    }
}
