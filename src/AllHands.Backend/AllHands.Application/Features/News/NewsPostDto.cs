using AllHands.Application.Dto;

namespace AllHands.Application.Features.News;

public sealed record NewsPostDto(
    Guid Id,
    string Text,
    DateTimeOffset CreatedAt,
    DateTimeOffset? EditedAt,
    EmployeeTitleDto? Author);
