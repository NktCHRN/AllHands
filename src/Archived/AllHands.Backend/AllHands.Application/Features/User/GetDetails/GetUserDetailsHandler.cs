using AllHands.Application.Abstractions;
using AllHands.Application.Dto;
using AllHands.Domain.Exceptions;
using AllHands.Domain.Models;
using Marten;
using MediatR;

namespace AllHands.Application.Features.User.GetDetails;

public sealed class GetUserDetailsHandler(
    ICurrentUserService currentUserService,
    IQuerySession querySession,
    IAccountService accountService) : IRequestHandler<GetUserDetailsQuery, EmployeeDetailsDto>
{
    public async Task<EmployeeDetailsDto> Handle(GetUserDetailsQuery request, CancellationToken cancellationToken)
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

        var role = await accountService.GetRoleByUserIdAsync(currentUser.Id, cancellationToken);

        return EmployeeDetailsDto.FromModel(employee, role);
    }
}
