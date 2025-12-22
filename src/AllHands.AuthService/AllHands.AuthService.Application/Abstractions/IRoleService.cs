using AllHands.AuthService.Application.Dto;
using AllHands.AuthService.Application.Features.Roles.Create;
using AllHands.AuthService.Application.Features.Roles.Get;
using AllHands.AuthService.Application.Features.Roles.GetById;
using AllHands.AuthService.Application.Features.Roles.GetUsersInRole;
using AllHands.AuthService.Application.Features.Roles.Update;
using AllHands.Shared.Application.Dto;

namespace AllHands.AuthService.Application.Abstractions;

public interface IRoleService
{
    Task<IReadOnlyList<RoleWithUsersCountDto>> GetAsync(CancellationToken cancellationToken);
    Task<PagedDto<EmployeeTitleDto>> GetUsersAsync(GetUsersInRoleQuery query, CancellationToken cancellationToken);
    Task<GetRoleByIdResult?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Guid> CreateAsync(CreateRoleCommand command, CancellationToken cancellationToken);
    Task UpdateAsync(UpdateRoleCommand command, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
    Task ResetUsersRoleAsync(Guid oldRoleId, CancellationToken cancellationToken);
    Task CreateDefaultRolesAsync(Guid companyId, CancellationToken cancellationToken);
}
