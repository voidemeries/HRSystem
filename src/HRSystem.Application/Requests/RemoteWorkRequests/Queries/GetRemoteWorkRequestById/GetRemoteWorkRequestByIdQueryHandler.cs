using HRSystem.Application.Common.Interfaces;
using HRSystem.Application.Requests.RemoteWorkRequests.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRSystem.Application.Requests.RemoteWorkRequests.Queries.GetRemoteWorkRequestById;

public class GetRemoteWorkRequestByIdQueryHandler : IRequestHandler<GetRemoteWorkRequestByIdQuery, RemoteWorkRequestDto>
{
    private readonly IApplicationDbContext _context;

    public GetRemoteWorkRequestByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<RemoteWorkRequestDto> Handle(GetRemoteWorkRequestByIdQuery request, CancellationToken cancellationToken)
    {
        var remoteWorkRequest = await _context.RemoteWorkRequests
            .Include(r => r.Requester)
            .Include(r => r.ForEmployee)
            .Include(r => r.ApproverPosition)
            .Include(r => r.Approver)
            .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Remote work request with ID {request.Id} not found.");

        return new RemoteWorkRequestDto
        {
            Id = remoteWorkRequest.Id,
            RequestType = remoteWorkRequest.RequestType.ToString(),
            RequesterId = remoteWorkRequest.RequesterId,
            RequesterName = $"{remoteWorkRequest.Requester.FirstName} {remoteWorkRequest.Requester.LastName}",
            ForEmployeeId = remoteWorkRequest.ForEmployeeId,
            ForEmployeeName = $"{remoteWorkRequest.ForEmployee.FirstName} {remoteWorkRequest.ForEmployee.LastName}",
            Status = remoteWorkRequest.Status.ToString(),
            ApproverPositionId = remoteWorkRequest.ApproverPositionId,
            ApproverPositionName = remoteWorkRequest.ApproverPosition.Name,
            ApproverId = remoteWorkRequest.ApproverId,
            ApproverName = remoteWorkRequest.Approver != null ? $"{remoteWorkRequest.Approver.FirstName} {remoteWorkRequest.Approver.LastName}" : null,
            ApprovalDate = remoteWorkRequest.ApprovalDate,
            RejectionReason = remoteWorkRequest.RejectionReason,
            SubmittedDate = remoteWorkRequest.SubmittedDate,
            StartDate = remoteWorkRequest.StartDate,
            EndDate = remoteWorkRequest.EndDate,
            Location = remoteWorkRequest.Location,
            Reason = remoteWorkRequest.Reason
        };
    }
}