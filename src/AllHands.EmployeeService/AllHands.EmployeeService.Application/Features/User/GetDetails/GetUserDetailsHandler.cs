using AllHands.EmployeeService.Application.Dto;
using AllHands.EmployeeService.Domain.Models;
using AllHands.Shared.Domain.Exceptions;
using AllHands.Shared.Domain.UserContext;
using Marten;
using MediatR;

namespace AllHands.EmployeeService.Application.Features.User.GetDetails;

public sealed class GetUserDetailsHandler(
    IUserContext userContext,
    IQuerySession querySession) : IRequestHandler<GetUserDetailsQuery, EmployeeDetailsDto>
{
    public async Task<EmployeeDetailsDto> Handle(GetUserDetailsQuery request, CancellationToken cancellationToken)
    {
        var userId = userContext.Id;

        Position position = null!;
        Employee manager = null!;
        Domain.Models.Company company = null!;
        Role role = null!;
        var employee = await querySession.Query<Domain.Models.Employee>()
                           .Include<Position>(x => position = x).On(x => x.PositionId)
                           .Include<Employee>(x => manager = x).On(x => x.ManagerId)
                           .Include<Domain.Models.Company>(x => company = x).On(x => x.CompanyId)
                           .Include<Role>(x => role = x).On(x => x.RoleId)
                           .FirstOrDefaultAsync(e => e.UserId == userId, token: cancellationToken)
                       ?? throw new EntityNotFoundException("User was not found");

        employee.Position = position!;

        employee.Manager = manager!;
        if (employee.Manager is not null)
        {
            employee.Manager.Position = await querySession.Query<Position>()
                .FirstOrDefaultAsync(x => x.Id == employee.Manager.PositionId,
                    token: cancellationToken);
        }

        employee.Company = company
                           ?? throw new EntityNotFoundException("Company was not found");

        employee.Role = role;

        return EmployeeDetailsDto.FromModel(employee);
    }
}
