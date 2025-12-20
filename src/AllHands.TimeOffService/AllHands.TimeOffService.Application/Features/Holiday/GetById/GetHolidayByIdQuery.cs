using AllHands.TimeOffService.Application.Dto;
using MediatR;

namespace AllHands.TimeOffService.Application.Features.Holiday.GetById;

public sealed record GetHolidayByIdQuery(Guid Id) : IRequest<HolidayDto>;
