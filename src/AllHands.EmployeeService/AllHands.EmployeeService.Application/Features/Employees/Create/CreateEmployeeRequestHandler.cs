using AllHands.EmployeeService.Application.Abstractions;
using AllHands.EmployeeService.Domain.Events.Employee;
using AllHands.EmployeeService.Domain.Models;
using AllHands.Shared.Domain.Exceptions;
using AllHands.Shared.Domain.UserContext;
using AllHands.Shared.Domain.Utilities;
using Marten;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AllHands.EmployeeService.Application.Features.Employees.Create;

public sealed class CreateEmployeeRequestHandler(IDocumentSession documentSession, IEventService eventService, IUserContext userContext, IUserClient userClient, ILogger<CreateEmployeeRequestHandler> logger) : IRequestHandler<CreateEmployeeCommand, CreateEmployeeResult>
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

        var positionExists = await documentSession.Query<Position>()
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

        var employeeId = Guid.CreateVersion7();
        var accountCreationResult = await userClient.CreateAsync(new CreateIdentityUserCommand(
            request.Email,
            request.FirstName,
            request.MiddleName,
            request.LastName,
            request.PhoneNumber,
            employeeId), cancellationToken);

        try
        {
            documentSession.Events.StartStream<Employee>(employeeId, new EmployeeCreatedEvent(
                employeeId,
                userContext.Id,
                accountCreationResult.UserId,
                userContext.CompanyId,
                request.PositionId,
                request.ManagerId,
                request.Email,
                normalizedEmail,
                request.FirstName,
                request.MiddleName,
                request.LastName,
                request.PhoneNumber,
                request.WorkStartDate));
            await eventService.PublishAsync(new Shared.Contracts.Messaging.Events.Employees.EmployeeCreatedEvent(
                employeeId,
                request.FirstName,
                request.MiddleName,
                request.LastName,
                request.Email,
                request.PhoneNumber,
                request.WorkStartDate,
                request.ManagerId,
                request.PositionId,
                userContext.CompanyId,
                accountCreationResult.UserId,
                nameof(EmployeeStatus.Unactivated)));
            await documentSession.SaveChangesAsync(cancellationToken);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occured while creating an employee.");
            await userClient.DeleteAsync(accountCreationResult.UserId, cancellationToken);

            throw;
        }

        return new CreateEmployeeResult(employeeId);
    }
}
