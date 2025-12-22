namespace AllHands.Application.Dto;

public sealed record PagedDto<TDto>(IReadOnlyList<TDto> Data, int TotalCount);
