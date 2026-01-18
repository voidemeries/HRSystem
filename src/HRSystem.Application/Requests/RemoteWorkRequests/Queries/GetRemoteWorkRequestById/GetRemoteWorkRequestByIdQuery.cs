using HRSystem.Application.Requests.RemoteWorkRequests.Common;
using MediatR;

namespace HRSystem.Application.Requests.RemoteWorkRequests.Queries.GetRemoteWorkRequestById;

public record GetRemoteWorkRequestByIdQuery(int Id) : IRequest<RemoteWorkRequestDto>;