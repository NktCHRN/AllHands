namespace AllHands.Shared.Domain.Abstractions;

public interface ISoftDeletableAuditable : ISoftDeletable
{
    Guid? DeletedByUserId { get; set; }
}
