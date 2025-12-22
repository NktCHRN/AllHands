using AllHands.EmployeeService.Domain.Events.Employee;
using AllHands.EmployeeService.Domain.Models;
using AllHands.Shared.Domain.Exceptions;
using AllHands.Shared.Domain.UserContext;
using Marten;
using MediatR;

namespace AllHands.EmployeeService.Application.Features.User.Update;

public sealed class UpdateUserHandler(IUserContext userContext, IDocumentSession documentSession) : IRequestHandler<UpdateUserCommand>
{
    public async Task Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var userId = userContext.Id;
        var employee = await documentSession.Query<Employee>()
            .Where(e => e.UserId == userId)
            .FirstOrDefaultAsync(cancellationToken);
        if (employee is null)
        {
            throw new EntityNotFoundException("User was not found");
        }
        
        await accountService.UpdateAsync(request, userId, cancellationToken);// TODO: gRPC call here

        documentSession.Events.Append(employee.Id, new EmployeeUpdatedBySelfEvent(
            employee.Id,
            userId,
            request.FirstName,
            request.MiddleName,
            request.LastName,
            request.PhoneNumber));
        await documentSession.SaveChangesAsync(cancellationToken);
    }
}
