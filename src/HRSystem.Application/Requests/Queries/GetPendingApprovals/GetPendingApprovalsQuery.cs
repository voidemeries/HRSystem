namespace HRSystem.Application.Requests.Queries.GetPendingApprovals;

using global::HRSystem.Application.Requests.Common;
using MediatR;

public record GetPendingApprovalsQuery : IRequest<List<PendingApprovalDto>>;