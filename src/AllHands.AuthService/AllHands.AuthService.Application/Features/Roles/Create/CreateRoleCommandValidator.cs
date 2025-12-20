using FluentValidation;

namespace AllHands.AuthService.Application.Features.Roles.Create;

public sealed class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
{
    public CreateRoleCommandValidator(RoleCommandBaseValidator baseValidator)
    {
        Include(baseValidator);
    }
}
