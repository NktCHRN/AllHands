using AllHands.Application.Abstractions;
using AllHands.Application.Dto;
using AllHands.Domain.Models;
using Marten;
using MediatR;

namespace AllHands.Application.Features.Roles.GetUsersInRole;

public sealed class GetUsersInRoleHandler(IRoleService roleService, IQuerySession querySession) : IRequestHandler<GetUsersInRoleQuery, PagedDto<EmployeeDto>>
{
    public async Task<PagedDto<EmployeeDto>> Handle(GetUsersInRoleQuery request, CancellationToken cancellationToken)
    {
        var users = await roleService.GetUsersAsync(request, cancellationToken);

        var usersIds = users.Data.Select(u => u.UserId).ToList();
        
        var positions = new Dictionary<Guid, Position>();
        var employees = await querySession.Query<Employee>()
            .Include(positions).On(u => u.PositionId)
            .Where(e => usersIds.Contains(e.UserId))
            .ToListAsync(cancellationToken);
        var employeesByUserId = employees.ToDictionary(e => e.UserId);

        return new PagedDto<EmployeeDto>(
            users
                .Data.Select(u =>
                {
                    var employee = employeesByUserId.GetValueOrDefault(u.UserId);
                    var position = positions.GetValueOrDefault(employee?.PositionId ?? Guid.Empty);
                    return new EmployeeDto()
                    {
                        Id = employee?.Id ?? Guid.Empty,
                        Email = u.Email,
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        MiddleName = u.MiddleName,
                        PhoneNumber = u.PhoneNumber,
                        Position = new PositionDto()
                        {
                            Id = position?.Id ?? Guid.Empty,
                            Name = position?.Name ?? string.Empty
                        }
                    };
                })
                .ToList(), users.TotalCount);
    }
}
