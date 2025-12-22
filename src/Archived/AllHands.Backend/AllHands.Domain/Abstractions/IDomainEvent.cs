namespace AllHands.Domain.Abstractions;

public interface IDomainEvent<out TStreamId>
{
    TStreamId StreamId { get; }
}