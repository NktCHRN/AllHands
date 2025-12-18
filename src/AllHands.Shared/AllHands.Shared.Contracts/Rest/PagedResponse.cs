namespace AllHands.Shared.Contracts.Rest;

public record PagedResponse<TResponse>(IReadOnlyList<TResponse> Data, int TotalCount)
{
}
