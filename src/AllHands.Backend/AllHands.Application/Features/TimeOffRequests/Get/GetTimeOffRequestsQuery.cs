using AllHands.Application.Dto;
using AllHands.Application.Queries;
using AllHands.Domain.Models;
using MediatR;

namespace AllHands.Application.Features.TimeOffRequests.Get;

public sealed record GetTimeOffRequestsQuery(int PerPage, int Page, Guid? ManagerId = null, Guid? EmployeeId = null, TimeOffRequestStatus Status = TimeOffRequestStatus.Undefined) : PagedQuery(PerPage, Page), IRequest<PagedDto<TimeOffRequestDto>>;
