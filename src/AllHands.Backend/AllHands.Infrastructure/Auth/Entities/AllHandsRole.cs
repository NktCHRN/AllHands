using Microsoft.AspNetCore.Identity;

namespace AllHands.Infrastructure.Auth.Entities;

public sealed class AllHandsRole : IdentityRole<Guid>
{
    public required Guid CompanyId { get; set; }
    public bool IsDefault { get; set; }
    
    public IList<AllHandsRoleClaim> Claims { get; set; } = [];
    public IList<AllHandsUserRole> Users { get; set; } = [];
    
    public required DateTimeOffset CreatedAt { get; set; }
    public Guid? CreatedByUserId { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public Guid? UpdatedByUserId { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public Guid? DeletedByUserId { get; set; }
}
