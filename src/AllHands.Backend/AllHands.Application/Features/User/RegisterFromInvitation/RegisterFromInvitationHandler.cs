using System.ComponentModel.DataAnnotations;
using AllHands.Application.Abstractions;
using AllHands.Domain.Events.Employee;
using AllHands.Domain.Exceptions;
using AllHands.Domain.Models;
using Marten;
using MediatR;

namespace AllHands.Application.Features.User.RegisterFromInvitation;

public sealed class RegisterFromInvitationHandler(IDocumentSession documentSession, IAccountService accountService) : IRequestHandler<RegisterFromInvitationCommand>
{
    public async Task Handle(RegisterFromInvitationCommand request, CancellationToken cancellationToken)
    {
        var userId = await accountService.RegisterFromInvitationAsync(request, cancellationToken);

        var employee = await documentSession.Query<Employee>()
            .Where(x => x.UserId == userId)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new EntityNotFoundException("Employee was not found");

        if (employee.Status != EmployeeStatus.Unactivated)
        {
            throw new ValidationException("The user is already activated.");
        }

        documentSession.Events.Append(employee.Id, cancellationToken, new EmployeeRegisteredEvent(employee.Id, employee.UserId));
        await documentSession.SaveChangesAsync(cancellationToken);
    }
}