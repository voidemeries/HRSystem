namespace HRSystem.Application.Requests.RemoteWorkRequests.Commands.ApproveRemoteWorkRequest;

using HRSystem.Application.Common.Interfaces;
using HRSystem.Application.Requests.RemoteWorkRequests.Common;
using HRSystem.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class ApproveRemoteWorkRequestCommandHandler : IRequestHandler<ApproveRemoteWorkRequestCommand, RemoteWorkRequestDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IApproverService _approverService;

    public ApproveRemoteWorkRequestCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        IApproverService approverService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _approverService = approverService;
    }

    public async Task<RemoteWorkRequestDto> Handle(ApproveRemoteWorkRequestCommand request, CancellationToken cancellationToken)
    {
        var currentEmployeeId = _currentUserService.EmployeeId
            ?? throw new UnauthorizedAccessException("User not authenticated.");

        var remoteWorkRequest = await _context.RemoteWorkRequests
            .Include(r => r.Requester)
            .Include(r => r.ForEmployee)
            .Include(r => r.ApproverPosition)
            .Include(r => r.Approver)
            .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Remote work request with ID {request.Id} not found.");

        if (remoteWorkRequest.Status != RequestStatus.Submitted)
        {
            throw new InvalidOperationException("Only submitted requests can be approved.");
        }

        var designatedApproverId = await _approverService.GetDesignatedApproverIdAsync(
            remoteWorkRequest.ApproverPositionId,
            cancellationToken);

        var isAuthorized = _currentUserService.IsAdmin || currentEmployeeId == designatedApproverId;

        if (!isAuthorized)
        {
            throw new UnauthorizedAccessException("You are not authorized to approve this request.");
        }

        remoteWorkRequest.Status = RequestStatus.Approved;
        remoteWorkRequest.ApproverId = currentEmployeeId;
        remoteWorkRequest.ApprovalDate = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

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