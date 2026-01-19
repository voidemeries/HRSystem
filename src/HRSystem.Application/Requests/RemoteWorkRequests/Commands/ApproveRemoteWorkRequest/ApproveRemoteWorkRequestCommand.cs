namespace HRSystem.Application.Requests.RemoteWorkRequests.Commands.ApproveRemoteWorkRequest;

using HRSystem.Application.Requests.RemoteWorkRequests.Common;
using MediatR;

public record ApproveRemoteWorkRequestCommand : IRequest<RemoteWorkRequestDto>
{
    public int Id { get; init; }
}