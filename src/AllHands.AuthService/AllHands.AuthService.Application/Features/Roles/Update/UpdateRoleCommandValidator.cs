using FluentValidation;

namespace AllHands.AuthService.Application.Features.Roles.Update;

public sealed class UpdateRoleCommandValidator : AbstractValidator<UpdateRoleCommand>
{
    public UpdateRoleCommandValidator(RoleCommandBaseValidator baseValidator)
    {
        Include(baseValidator);
    }
}
