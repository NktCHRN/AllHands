using AllHands.Application.Abstractions;
using AllHands.Application.Dto;
using AllHands.Domain.Exceptions;
using AllHands.Domain.Models;
using Marten;
using MediatR;

namespace AllHands.Application.Features.User.GetDetails;

public sealed class GetUserDetailsHandler(ICurrentUserService currentUserService, IQuerySession querySession, IAccountService accountService) : IRequestHandler<GetUserDetailsQuery, GetUserDetailsResult>
{
    public async Task<GetUserDetailsResult> Handle(GetUserDetailsQuery request, CancellationToken cancellationToken)
    {
        var currentUser = currentUserService.GetCurrentUser();

        Position position = null!;
        Employee manager = null!;
        Domain.Models.Company company = null!;
        var employee = await querySession.Query<Domain.Models.Employee>()
                           .Include<Position>(x => position = x).On(x => x.PositionId)
                           .Include<Employee>(x => manager = x).On(x => x.ManagerId)
                           .Include<Domain.Models.Company>(x => company = x).On(x => x.CompanyId)
                           .FirstOrDefaultAsync(e => e.UserId == currentUser.Id, token: cancellationToken)
                       ?? throw new EntityNotFoundException("User was not found");

        employee.Position = position
                            ?? throw new EntityNotFoundException("Position was not found");

        employee.Manager = manager
                           ?? throw new EntityNotFoundException("Manager was not found");
        employee.Manager.Position = await querySession.Query<Position>()
            .FirstOrDefaultAsync(x => x.Id == employee.Manager.PositionId, token: cancellationToken)
                                    ?? throw new EntityNotFoundException("Manager position was not found");

        employee.Company = company
                           ?? throw new EntityNotFoundException("Company was not found");
        
        var role = await accountService.GetRoleByUserIdAsync(currentUser.Id, cancellationToken);

        return new GetUserDetailsResult(
            employee.Id,
            employee.FirstName,
            employee.MiddleName,
            employee.LastName,
            employee.Email,
            employee.PhoneNumber,
            employee.WorkStartDate,
            new EmployeeDto{
        Id = employee.Manager.Id,
        FirstName = employee.Manager.FirstName,
        MiddleName = employee.Manager.MiddleName,
        LastName = employee.Manager.LastName,
        Email = employee.Manager.Email,
        PhoneNumber = employee.Manager.PhoneNumber,
        Position = new PositionDto
        {
            Id = employee.Manager.PositionId,
            Name = employee.Manager.Position.Name
        }
    },
    new PositionDto
            {
                Id = employee.PositionId, 
                Name = employee.Position.Name
            },
            new CompanyDto()
            {
                Id = employee.Company.Id,
                Name = employee.Company.Name
            }, 
            role == null ? null! : new RoleDto(role.Id, role.Name, role.IsDefault));
    }
}
