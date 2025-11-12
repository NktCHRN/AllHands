namespace AllHands.Infrastructure.Auth;

public sealed class SessionCache
{
    public required Guid Key { get; set; }
    public required string TicketValue { get; set; } = string.Empty;
    public required DateTimeOffset? IssuesAt { get; set; }
    public required DateTimeOffset? ExpiresAt { get; set; }
    public required Guid UserId { get; set; }
}
