using AllHands.Shared.Application.Dto;
using AllHands.Shared.Application.Queries;
using AllHands.TimeOffService.Domain.Models;
using MediatR;

namespace AllHands.TimeOffService.Application.Features.TimeOffRequests.Get;

public sealed record GetTimeOffRequestsQuery(int PerPage, int Page, Guid? ManagerId = null, Guid? EmployeeId = null, TimeOffRequestStatus Status = TimeOffRequestStatus.Undefined) : PagedQuery(PerPage, Page), IRequest<PagedDto<TimeOffRequestDto>>;
