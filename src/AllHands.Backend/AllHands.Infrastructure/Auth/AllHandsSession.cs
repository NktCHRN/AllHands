namespace AllHands.Infrastructure.Auth;

public sealed class AllHandsSession
{
    public required string Key { get; set; }
    public required string TicketValue { get; set; } = string.Empty;
    public required DateTimeOffset CreatedAt { get; set; }
    public required DateTimeOffset ExpiresAt { get; set; }
    public required Guid UserId { get; set; }
    public required AllHandsIdentityUser? User { get; set; }
}
