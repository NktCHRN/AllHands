using AllHands.Application.Abstractions;
using AllHands.Application.Features.Employees.Create;
using AllHands.Domain.Exceptions;
using AllHands.Domain.Models;
using AllHands.Domain.Utilities;
using Marten;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AllHands.Application.Features.Employees.ResendInvitation;

public sealed class ResendInvitationHandler(IDocumentSession documentSession, IAccountService accountService, ICurrentUserService currentUserService, ILogger<ResendInvitationHandler> logger, IEmailSender emailSender) : IRequestHandler<ResendInvitationCommand>
{
    public async Task Handle(ResendInvitationCommand request, CancellationToken cancellationToken)
    {
        var employee = await documentSession.Query<Employee>()
            .FirstOrDefaultAsync(e => e.Id == request.EmployeeId, cancellationToken)
            ?? throw new EntityNotFoundException("Employee was not found.");

        if (employee.Status != EmployeeStatus.Unactivated)
        {
            throw new EntityAlreadyExistsException("Employee was already activated.");
        }
        
        var invitationGenerationResult = await accountService.RegenerateInvitationAsync(employee.UserId, cancellationToken);
        
        try
        {
            var admin = currentUserService.GetCurrentUser();
            await emailSender.SendCompleteRegistrationEmailAsync(new SendCompleteRegistrationEmailCommand(
                employee.Email,
                employee.FirstName,
                StringUtilities.GetFullName(admin.FirstName, admin.MiddleName, admin.LastName),
                invitationGenerationResult.Id,
                invitationGenerationResult.Token), cancellationToken);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while sending registration email.");
            throw;
        }
    }
}
