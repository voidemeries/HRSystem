using HRSystem.Application.Requests.RemoteWorkRequests.Common;
using HRSystem.Domain.Enums;
using MediatR;

namespace HRSystem.Application.Requests.RemoteWorkRequests.Queries.GetRemoteWorkRequests;

public record GetRemoteWorkRequestsQuery : IRequest<List<RemoteWorkRequestDto>>
{
    public bool? Mine { get; init; }
    public int? ForEmployeeId { get; init; }
    public RequestStatus? Status { get; init; }
}