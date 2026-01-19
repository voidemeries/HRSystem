namespace HRSystem.Application.Requests.Queries.GetMyApprovals;

using HRSystem.Application.Requests.Common;
using HRSystem.Domain.Enums;
using MediatR;

public record GetMyApprovalsQuery : IRequest<List<PendingApprovalDto>>
{
    public RequestStatus? Status { get; init; }
}