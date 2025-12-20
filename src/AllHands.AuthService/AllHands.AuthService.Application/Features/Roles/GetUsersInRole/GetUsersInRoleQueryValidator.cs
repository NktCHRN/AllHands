using AllHands.Shared.Application.Validation;
using FluentValidation;

namespace AllHands.AuthService.Application.Features.Roles.GetUsersInRole;

public sealed class GetUsersInRoleQueryValidator : AbstractValidator<GetUsersInRoleQuery>
{
    public GetUsersInRoleQueryValidator(BasePagedQueryValidator baseValidator)
    {
        Include(baseValidator);
    }
}
