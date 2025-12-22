using AllHands.EmployeeService.Domain.Events.Employee;
using AllHands.EmployeeService.Domain.Models;
using AllHands.Shared.Domain.Exceptions;
using AllHands.Shared.Domain.UserContext;
using Marten;
using MediatR;

namespace AllHands.EmployeeService.Application.Features.User.Reactivate;

public sealed class ReactivateUserCommandHandler(IDocumentSession documentSession, IUserContext userContext) : IRequestHandler<ReactivateUserCommand>
{
    public async Task Handle(ReactivateUserCommand request, CancellationToken cancellationToken)
    {
        var employee = await documentSession.Query<Employee>()
            .FirstOrDefaultAsync(x => x.UserId == request.UserId, token: cancellationToken)
            ?? throw new EntityNotFoundException("Employee was not found");
        
        documentSession.Events.Append(employee.Id, new EmployeeReactivatedEvent(employee.Id, userContext.Id, request.GlobalUserId, request.RoleIds.First()));
        
        await documentSession.SaveChangesAsync(cancellationToken);
    }
}
