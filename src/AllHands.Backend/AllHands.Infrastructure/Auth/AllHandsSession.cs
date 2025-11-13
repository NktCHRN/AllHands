namespace AllHands.Infrastructure.Auth;

public sealed class AllHandsSession
{
    public required Guid Key { get; set; }      // Guid v4, not sequential GUID here.
    public required byte[] TicketValue { get; set; }
    public required DateTimeOffset? IssuedAt { get; set; }
    public required DateTimeOffset? ExpiresAt { get; set; }
    public required Guid UserId { get; set; }
    public AllHandsIdentityUser? User { get; set; }
}
