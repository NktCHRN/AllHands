using AllHands.Application.Dto;
using AllHands.Application.Features.Roles.Create;
using AllHands.Application.Features.Roles.Get;
using AllHands.Application.Features.Roles.GetById;
using AllHands.Application.Features.Roles.GetUsersInRole;
using AllHands.Application.Features.Roles.Update;

namespace AllHands.Application.Abstractions;

public interface IRoleService
{
    Task<IReadOnlyList<RoleWithUsersCountDto>> GetAsync(CancellationToken cancellationToken);
    Task<PagedDto<UserDto>> GetUsersAsync(GetUsersInRoleQuery query, CancellationToken cancellationToken);
    Task<GetRoleByIdResult?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Guid> CreateAsync(CreateRoleCommand command, CancellationToken cancellationToken);
    Task UpdateAsync(UpdateRoleCommand command, CancellationToken cancellationToken);
}
