using Microsoft.AspNetCore.Identity;

namespace AllHands.Infrastructure.Auth.Entities;

public sealed class AllHandsIdentityUser : IdentityUser<Guid>
{
    public string FirstName { get; internal set; } = string.Empty;
    public string? MiddleName { get; internal set; }
    public string LastName { get; internal set; } = string.Empty;
    public required Guid CompanyId { get; set; }
    
    public IList<Invitation> IssuedInvitations { get; set; } = [];
    public IList<Invitation> Invitations { get; set; } = [];
    public IList<AllHandsUserRole> Roles { get; set; } = [];
}
