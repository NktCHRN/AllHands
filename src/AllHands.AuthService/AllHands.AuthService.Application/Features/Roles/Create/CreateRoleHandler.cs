using AllHands.AuthService.Application.Abstractions;
using AllHands.Shared.Application.Dto;
using MediatR;

namespace AllHands.AuthService.Application.Features.Roles.Create;

public sealed class CreateRoleHandler(IRoleService roleService) : IRequestHandler<CreateRoleCommand, CreatedEntityDto>
{
    public async Task<CreatedEntityDto> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        var id = await roleService.CreateAsync(request, cancellationToken);
        return new CreatedEntityDto(id);
    }
}
