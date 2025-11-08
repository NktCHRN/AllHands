namespace AllHands.Domain.Models;

public sealed class TimeOffType
{
    public required Guid Id { get; set; }
    public required Guid CompanyId { get; set; }
    public required string Name { get; set; }
    public required string Emoji { get; set; }
    public required DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
}
