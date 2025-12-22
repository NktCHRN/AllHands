using AllHands.EmployeeService.Domain.Events.Employee;
using AllHands.EmployeeService.Domain.Models;
using AllHands.Shared.Domain.Exceptions;
using AllHands.Shared.Domain.UserContext;
using Marten;
using MediatR;

namespace AllHands.EmployeeService.Application.Features.Employees.Delete;

public sealed class DeleteEmployeeHandler(IDocumentSession documentSession, IUserContext userContext) : IRequestHandler<DeleteEmployeeCommand>
{
    public async Task Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employee = await documentSession.Query<Employee>()
                           .FirstOrDefaultAsync(e => e.Id == request.EmployeeId, cancellationToken)
                       ?? throw new EntityNotFoundException("Employee was not found.");
        
        // TODO: send event
        
        documentSession.Events.Append(request.EmployeeId, new EmployeeDeletedEvent(request.EmployeeId, userContext.Id, request.Reason));
        await documentSession.SaveChangesAsync(cancellationToken);
    }
}
