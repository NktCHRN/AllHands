using Microsoft.AspNetCore.Identity;

namespace AllHands.AuthService.Domain.Models;

public sealed class AllHandsIdentityUser : IdentityUser<Guid>
{
    public string FirstName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string LastName { get; set; } = string.Empty;
    public required Guid CompanyId { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public DateTimeOffset? DeactivatedAt { get; set; }
    public DateTimeOffset? LastPasswordResetRequestedAt { get; set; }
    
    public bool IsInvitationAccepted { get; set; }
    
    public Guid GlobalUserId { get; set; }
    public AllHandsGlobalUser? GlobalUser { get; set; }
    
    public Guid EmployeeId { get; set; }
    
    public IList<Invitation> IssuedInvitations { get; set; } = [];
    public IList<Invitation> Invitations { get; set; } = [];
    public IList<AllHandsUserRole> Roles { get; set; } = [];
}
