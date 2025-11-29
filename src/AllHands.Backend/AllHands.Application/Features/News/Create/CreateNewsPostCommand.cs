using AllHands.Application.Dto;
using MediatR;

namespace AllHands.Application.Features.News.Create;

public sealed record CreateNewsPostCommand(string Text) : NewsCommandBase(Text), IRequest<CreatedEntityDto>;
