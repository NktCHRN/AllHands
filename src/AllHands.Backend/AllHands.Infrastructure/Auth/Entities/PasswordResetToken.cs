namespace AllHands.Infrastructure.Auth.Entities;

public sealed class PasswordResetToken
{
    public required Guid Id { get; set; }
    public required string TokenHash { get; set; }
    public required DateTimeOffset IssuedAt { get; set; }
    public required DateTimeOffset ExpiresAt { get; set; }
    public bool IsUsed { get; set; }
    
    public Guid GlobalUserId { get; set; }
    public AllHandsGlobalUser? GlobalUser { get; set; }
}
