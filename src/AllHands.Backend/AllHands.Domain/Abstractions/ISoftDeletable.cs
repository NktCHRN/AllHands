namespace AllHands.Domain.Abstractions;

public interface ISoftDeletable
{
    DateTimeOffset? DeletedAt { get; }
}
