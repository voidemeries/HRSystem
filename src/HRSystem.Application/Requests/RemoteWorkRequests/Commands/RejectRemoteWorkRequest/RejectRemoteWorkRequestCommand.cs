namespace HRSystem.Application.Requests.RemoteWorkRequests.Commands.RejectRemoteWorkRequest;

using HRSystem.Application.Requests.RemoteWorkRequests.Common;
using MediatR;

public record RejectRemoteWorkRequestCommand : IRequest<RemoteWorkRequestDto>
{
    public int Id { get; init; }
    public string RejectionReason { get; init; } = string.Empty;
}