using AllHands.Application.Abstractions;
using AllHands.Domain.Events.Employee;
using AllHands.Domain.Exceptions;
using AllHands.Domain.Models;
using AllHands.Domain.Utilities;
using Marten;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AllHands.Application.Features.Employees.Create;

public sealed class CreateEmployeeRequestHandler(IDocumentSession documentSession, IAccountService accountService, ICurrentUserService currentUserService, IEmailSender emailSender, ILogger<CreateEmployeeRequestHandler> logger) : IRequestHandler<CreateEmployeeCommand, CreateEmployeeResult>
{
    public async Task<CreateEmployeeResult> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var company = await documentSession.Query<Domain.Models.Company>()
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new EntityNotFoundException("Company was not found.");

        if (company.IsSameDomainValidationEnforced)
        {
            var userEmailDomain = request.Email[(request.Email.IndexOf('@')+1)..];
            if (!string.Equals(company.EmailDomain.Replace("@", ""), userEmailDomain,
                    StringComparison.InvariantCultureIgnoreCase))
            {
                throw new EntityValidationFailedException("Email did not match with company domain.");
            }
        }

        var positionExists = await documentSession.Query<Domain.Models.Position>()
            .AnyAsync(p => p.Id == request.PositionId, cancellationToken);
        if (!positionExists)
        {
            throw new EntityNotFoundException("Position was not found.");
        }
        
        var managerExists = await documentSession.Query<Domain.Models.Employee>()
            .AnyAsync(p => p.Id == request.ManagerId, cancellationToken);
        if (!managerExists)
        {
            throw new EntityNotFoundException("Manager was not found.");
        }

        var normalizedEmail = StringUtilities.GetNormalizedEmail(request.Email);
        var employeeIsFired = await documentSession.Query<Employee>()
            .AnyAsync(e => e.NormalizedEmail == normalizedEmail && e.Status == EmployeeStatus.Fired, cancellationToken);
        if (employeeIsFired)
        {
            throw new EntityAlreadyExistsException("Employee already worked here, he is listed as a fired employee. Please, find and rehire them instead.");
        }

        var accountCreationResult = await accountService.CreateAsync(request, cancellationToken);

        var employeeId = Guid.CreateVersion7();
        documentSession.Events.StartStream(employeeId, new EmployeeCreatedEvent(
            employeeId,
            currentUserService.GetId(),
            accountCreationResult.Id,
            currentUserService.GetCompanyId(),
            request.PositionId,
            request.ManagerId,
            request.Email,
            normalizedEmail,
            request.FirstName,
            request.MiddleName,
            request.LastName,
            request.PhoneNumber,
            request.WorkStartDate));
        await documentSession.SaveChangesAsync(cancellationToken);

        var emailSentSuccessfully = false;
        try
        {
            var admin = currentUserService.GetCurrentUser();
            await emailSender.SendCompleteRegistrationEmailAsync(new SendCompleteRegistrationEmailCommand(
                request.Email,
                request.FirstName,
                StringUtilities.GetFullName(admin.FirstName, admin.MiddleName, admin.LastName),
                accountCreationResult.InvitationId,
                accountCreationResult.InvitationToken), cancellationToken);
            emailSentSuccessfully = true;
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while sending registration email.");
        }

        return new CreateEmployeeResult(employeeId, emailSentSuccessfully);
    }
}
