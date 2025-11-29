using Microsoft.AspNetCore.Identity;

namespace AllHands.Infrastructure.Auth.Entities;

public sealed class AllHandsUserRole : IdentityUserRole<Guid>
{
    public AllHandsRole? Role { get; set; }
    public AllHandsIdentityUser? User { get; set; }
}
