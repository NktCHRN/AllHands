using Microsoft.AspNetCore.Identity;

namespace AllHands.AuthService.Domain.Models;

public sealed class AllHandsUserRole : IdentityUserRole<Guid>
{
    public AllHandsRole? Role { get; set; }
    public AllHandsIdentityUser? User { get; set; }
}
