using AllHands.NewsService.Application.Dto;

namespace AllHands.NewsService.Application.Features.News;

public sealed record NewsPostDto(
    Guid Id,
    string Text,
    DateTimeOffset CreatedAt,
    DateTimeOffset? EditedAt,
    EmployeeTitleDto? Author);
