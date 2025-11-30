using AllHands.Application.Dto;
using MediatR;

namespace AllHands.Application.Features.Holiday.GetById;

public sealed record GetHolidayByIdQuery(Guid Id) : IRequest<HolidayDto>;
