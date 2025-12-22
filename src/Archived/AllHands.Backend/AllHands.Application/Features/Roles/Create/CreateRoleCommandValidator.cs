using FluentValidation;

namespace AllHands.Application.Features.Roles.Create;

public sealed class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
{
    public CreateRoleCommandValidator(RoleCommandBaseValidator baseValidator)
    {
        Include(baseValidator);
    }
}
