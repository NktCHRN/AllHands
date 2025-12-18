using AllHands.Shared.Application.Dto;
using MediatR;

namespace AllHands.NewsService.Application.Features.News.Create;

public sealed record CreateNewsPostCommand(string Text) : NewsCommandBase(Text), IRequest<CreatedEntityDto>;
