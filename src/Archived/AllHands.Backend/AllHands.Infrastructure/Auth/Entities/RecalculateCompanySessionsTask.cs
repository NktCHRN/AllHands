namespace AllHands.Infrastructure.Auth.Entities;

public sealed class RecalculateCompanySessionsTask
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public Guid RequesterUserId { get; set; }
    public DateTimeOffset RequestedAt { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }
    public int FailedAttempts { get; set; }
}
