using Microsoft.AspNetCore.Identity;

namespace AllHands.Infrastructure.Auth.Entities;

public sealed class AllHandsRole : IdentityRole<Guid>
{
    public required Guid CompanyId { get; set; }
    public IList<IdentityRoleClaim<Guid>> Claims { get; set; } = [];
}
