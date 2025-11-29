using AllHands.Application.Abstractions;
using AllHands.Application.Dto;
using MediatR;

namespace AllHands.Application.Features.Roles.Create;

public sealed class CreateRoleHandler(IRoleService roleService) : IRequestHandler<CreateRoleCommand, CreatedEntityDto>
{
    public async Task<CreatedEntityDto> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        var id = await roleService.CreateRoleAsync(request, cancellationToken);
        return new CreatedEntityDto(id);
    }
}
