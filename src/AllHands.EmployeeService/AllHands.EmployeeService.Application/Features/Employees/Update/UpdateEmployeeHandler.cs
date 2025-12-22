using AllHands.EmployeeService.Domain.Events.Employee;
using AllHands.EmployeeService.Domain.Models;
using AllHands.Shared.Domain.Exceptions;
using AllHands.Shared.Domain.Utilities;
using Marten;
using MediatR;

namespace AllHands.EmployeeService.Application.Features.Employees.Update;

public sealed class UpdateEmployeeHandler(IDocumentSession documentSession, IAccountService accountService, ICurrentUserService currentUserService) : IRequestHandler<UpdateEmployeeCommand>
{
    public async Task Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employee = await documentSession.Query<Employee>()
                           .FirstOrDefaultAsync(e => e.Id == request.EmployeeId, cancellationToken)
                       ?? throw new EntityNotFoundException("Employee was not found.");

        if (employee.Status != EmployeeStatus.Active && employee.Status != EmployeeStatus.Unactivated)
        {
            throw new EntityAlreadyExistsException("Cannot update fired employee.");
        }

        if (employee.PositionId != request.PositionId)
        {
            if (!await documentSession.Query<Position>().AnyAsync(e => e.Id == request.PositionId, cancellationToken))
            {
                throw new EntityNotFoundException("Position was not found.");
            }
        }

        if (employee.ManagerId != request.ManagerId)
        {
            if (!await documentSession.Query<Employee>().AnyAsync(e => e.Id == request.ManagerId, cancellationToken))
            {
                throw new EntityNotFoundException("Manager was not found.");
            }
        }
        
        await accountService.UpdateAsync(request, employee.UserId, cancellationToken);

        documentSession.Events.Append(employee.Id, new EmployeeUpdatedEvent(
            employee.Id,
            currentUserService.GetId(),
            request.PositionId,
            request.ManagerId,
            request.Email,
            StringUtilities.GetNormalizedEmail(request.Email),
            request.FirstName,
            request.MiddleName,
            request.LastName,
            request.PhoneNumber,
            request.WorkStartDate));
        await documentSession.SaveChangesAsync(cancellationToken);
    }
}
