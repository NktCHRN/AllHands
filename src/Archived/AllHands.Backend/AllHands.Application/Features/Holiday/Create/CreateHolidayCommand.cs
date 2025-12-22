using AllHands.Application.Dto;
using MediatR;

namespace AllHands.Application.Features.Holiday.Create;

public sealed record CreateHolidayCommand(string Name, DateOnly Date) : HolidayCommandBase(Name, Date), IRequest<CreatedEntityDto>;