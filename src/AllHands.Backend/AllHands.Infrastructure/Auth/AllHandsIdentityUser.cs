using Microsoft.AspNetCore.Identity;

namespace AllHands.Infrastructure.Auth;

public sealed class AllHandsIdentityUser : IdentityUser<Guid>
{
    public IList<Invitation> IssuedInvitations { get; set; } = [];
    public IList<Invitation> Invitations { get; set; } = [];
}
