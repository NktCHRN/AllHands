namespace AllHands.Infrastructure.Auth.Entities;

public sealed class AllHandsGlobalUser
{
    public required Guid Id { get; set; }
    public required string Email { get; set; }
    public required string NormalizedEmail { get; set; }
    public Guid DefaultCompanyId {get; set;}

    public IList<AllHandsIdentityUser> Users { get; set; } = [];
    public IList<PasswordResetToken> PasswordResetTokens { get; set; } = [];
}
