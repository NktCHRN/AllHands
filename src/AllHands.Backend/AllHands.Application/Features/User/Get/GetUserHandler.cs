using AllHands.Application.Abstractions;
using AllHands.Application.Features.User.Dto;
using AllHands.Domain.Exceptions;
using AllHands.Domain.Models;
using Marten;
using MediatR;

namespace AllHands.Application.Features.User.Get;

public sealed class GetUserHandler(ICurrentUserService currentUserService, IQuerySession querySession) : IRequestHandler<GetUserQuery, GetUserResult>
{
    public async Task<GetUserResult> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        var currentUser = currentUserService.GetCurrentUser();

        var employee = await querySession.Query<Domain.Models.Employee>()
                           .FirstOrDefaultAsync(e => e.UserId == currentUser.Id, token: cancellationToken)
                       ?? throw new EntityNotFoundException("User was not found");

        employee.Position = await querySession.Query<Position>()
                                .FirstOrDefaultAsync(x => x.Id == employee.PositionId, cancellationToken)
                            ?? throw new EntityNotFoundException("Position was not found");

        var roles = currentUserService.GetRoles();
        var permissions = currentUserService.GetPermissions();
        
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
                Name = employee.Position.Name
            },
            roles,
            permissions);
    }
}