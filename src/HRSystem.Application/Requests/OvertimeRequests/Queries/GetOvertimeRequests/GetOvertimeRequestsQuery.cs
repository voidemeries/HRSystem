using HRSystem.Application.Requests.OvertimeRequests.Common;
using HRSystem.Domain.Enums;
using MediatR;

namespace HRSystem.Application.Requests.OvertimeRequests.Queries.GetOvertimeRequests;

public record GetOvertimeRequestsQuery : IRequest<List<OvertimeRequestDto>>
{
    public bool? Mine { get; init; }
    public int? ForEmployeeId { get; init; }
    public RequestStatus? Status { get; init; }
}