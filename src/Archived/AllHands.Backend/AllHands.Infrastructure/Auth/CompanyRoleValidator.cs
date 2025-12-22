using AllHands.Infrastructure.Auth.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AllHands.Infrastructure.Auth;

public sealed class CompanyRoleValidator
    : IRoleValidator<AllHandsRole>
{
    public async Task<IdentityResult> ValidateAsync(
        RoleManager<AllHandsRole> manager,
        AllHandsRole role)
    {
        var normalizedName = manager.NormalizeKey(role.Name);
        var exists = await manager.Roles.AnyAsync(r =>
            r.CompanyId == role.CompanyId &&
            r.NormalizedName == normalizedName &&
            r.DeletedAt == null &&
            r.Id != role.Id);

        if (exists)
        {
            return IdentityResult.Failed(new IdentityError
            {
                Code = "DuplicateRole",
                Description = $"Role '{role.Name}' already exists in this company."
            });
        }

        return IdentityResult.Success;
    }
}
