using AllHands.Application.Abstractions;
using AllHands.Domain.Events.Employee;
using AllHands.Domain.Exceptions;
using AllHands.Domain.Models;
using Marten;
using MediatR;

namespace AllHands.Application.Features.User.Update;

public sealed class UpdateUserHandler(ICurrentUserService currentUserService, IAccountService accountService, IDocumentSession documentSession) : IRequestHandler<UpdateUserCommand>
{
    public async Task Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetId();
        var employee = await documentSession.Query<Employee>()
            .Where(e => e.UserId == userId)
            .FirstOrDefaultAsync(cancellationToken);
        if (employee is null)
        {
            throw new EntityNotFoundException("User was not found");
        }
        
        await accountService.Update(request, userId, cancellationToken);

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
