using AllHands.EmployeeService.Application.Dto;
using AllHands.EmployeeService.Domain.Models;
using AllHands.Shared.Application.Auth;
using AllHands.Shared.Domain.Exceptions;
using AllHands.Shared.Domain.UserContext;
using Marten;
using MediatR;

namespace AllHands.EmployeeService.Application.Features.User.Get;

public sealed class GetUserHandler(IUserContext userContext, IQuerySession querySession, IUserPermissionService userPermissionService) : IRequestHandler<GetUserQuery, GetUserResult>
{
    public async Task<GetUserResult> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        var employee = await querySession.Query<Employee>()
                           .FirstOrDefaultAsync(e => e.UserId == userContext.Id, token: cancellationToken)
                       ?? throw new EntityNotFoundException("User was not found");

        employee.Position = await querySession.Query<Position>()
            .FirstOrDefaultAsync(x => x.Id == employee.PositionId, cancellationToken);

        var roles = userContext.Roles;
        var permissions = userPermissionService.GetPermissions();
        
        return new GetUserResult(
            employee.Id,
            employee.FirstName,
            employee.MiddleName,
            employee.LastName,
            employee.Email,
            employee.PhoneNumber,
            new PositionDto
            {
                Id = employee.PositionId, 
                Name = employee.Position?.Name ?? string.Empty
            },
            roles,
            permissions);
    }
}
