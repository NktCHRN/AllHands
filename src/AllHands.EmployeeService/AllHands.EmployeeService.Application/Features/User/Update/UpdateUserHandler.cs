using AllHands.EmployeeService.Application.Abstractions;
using AllHands.EmployeeService.Application.Features.Employees.Update;
using AllHands.EmployeeService.Domain.Events.Employee;
using AllHands.EmployeeService.Domain.Models;
using AllHands.Shared.Domain.Exceptions;
using AllHands.Shared.Domain.UserContext;
using Marten;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AllHands.EmployeeService.Application.Features.User.Update;

public sealed class UpdateUserHandler(IUserContext userContext, IEventService eventService, IDocumentSession documentSession, IUserClient userClient, ILogger<UpdateUserHandler> logger) : IRequestHandler<UpdateUserCommand>
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
        
        var updateResult = await userClient.UpdateAsync(new UpdateIdentityUserCommand(
            employee.UserId,
            employee.Email,
            request.FirstName,
            request.MiddleName,
            request.LastName,
            request.PhoneNumber,
            null), cancellationToken);

        try
        {
            documentSession.Events.Append(employee.Id, new EmployeeUpdatedBySelfEvent(
                employee.Id,
                userId,
                request.FirstName,
                request.MiddleName,
                request.LastName,
                request.PhoneNumber,
                updateResult.GlobalUserId,
                updateResult.RoleId));
            await eventService.PublishAsync(new Shared.Contracts.Messaging.Events.Employees.EmployeeUpdatedEvent(
                employee.Id,
                request.FirstName,
                request.MiddleName,
                request.LastName,
                employee.Email,
                request.PhoneNumber,
                employee.WorkStartDate,
                employee.ManagerId,
                employee.PositionId,
                userContext.CompanyId,
                employee.UserId));
            await documentSession.SaveChangesAsync(cancellationToken);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while updating employee (account).");
            await userClient.UpdateAsync(new UpdateIdentityUserCommand(
                employee.UserId,
                employee.Email,
                employee.FirstName,
                employee.MiddleName,
                employee.LastName,
                employee.PhoneNumber,
                null), cancellationToken);
            throw;
        }

    }
}
