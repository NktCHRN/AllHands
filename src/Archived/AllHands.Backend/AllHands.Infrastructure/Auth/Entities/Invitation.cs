namespace AllHands.Infrastructure.Auth.Entities;

public sealed class Invitation
{
    public required Guid Id { get; set; }
    public required string TokenHash { get; set; }
    public required DateTimeOffset IssuedAt { get; set; }
    public required DateTimeOffset ExpiresAt { get; set; }
    public bool IsUsed { get; set; }
    public required Guid IssuerId { get; set; }
    public AllHandsIdentityUser? Issuer { get; set; }
    public required Guid UserId { get; set; }
    public AllHandsIdentityUser? User { get; set; }
}
