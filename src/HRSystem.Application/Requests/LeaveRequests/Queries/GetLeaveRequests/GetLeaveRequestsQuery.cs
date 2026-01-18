namespace HRSystem.Application.Requests.LeaveRequests.Queries.GetLeaveRequests;

using HRSystem.Application.Requests.LeaveRequests.Common;
using HRSystem.Domain.Enums;
using MediatR;

public record GetLeaveRequestsQuery : IRequest<List<LeaveRequestDto>>
{
    public bool? Mine { get; init; }
    public int? ForEmployeeId { get; init; }
    public RequestStatus? Status { get; init; }
}