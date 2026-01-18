using HRSystem.Application.Requests.RemoteWorkRequests.Common;
using MediatR;

namespace HRSystem.Application.Requests.RemoteWorkRequests.Commands.CreateRemoteWorkRequest;

public record CreateRemoteWorkRequestCommand : IRequest<RemoteWorkRequestDto>
{
    public int? ForEmployeeId { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public string Location { get; init; } = string.Empty;
    public string Reason { get; init; } = string.Empty;
}