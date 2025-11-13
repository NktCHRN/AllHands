using Microsoft.AspNetCore.Identity;

namespace AllHands.Infrastructure.Auth.Entities;

public sealed class AllHandsIdentityUser : IdentityUser<Guid>
{
    public required Guid CompanyId { get; set; }
    public IList<Invitation> IssuedInvitations { get; set; } = [];
    public IList<Invitation> Invitations { get; set; } = [];
}
