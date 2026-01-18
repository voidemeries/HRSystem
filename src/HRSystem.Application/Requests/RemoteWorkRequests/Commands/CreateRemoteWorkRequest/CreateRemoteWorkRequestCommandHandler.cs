namespace HRSystem.Application.Requests.RemoteWorkRequests.Commands.CreateRemoteWorkRequest;

using HRSystem.Application.Common.Interfaces;
using HRSystem.Application.Requests.RemoteWorkRequests.Common;
using HRSystem.Domain.Entities;
using HRSystem.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class CreateRemoteWorkRequestCommandHandler : IRequestHandler<CreateRemoteWorkRequestCommand, RemoteWorkRequestDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IApproverService _approverService;

    public CreateRemoteWorkRequestCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        IApproverService approverService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _approverService = approverService;
    }

    public async Task<RemoteWorkRequestDto> Handle(CreateRemoteWorkRequestCommand request, CancellationToken cancellationToken)
    {
        var currentEmployeeId = _currentUserService.EmployeeId
            ?? throw new UnauthorizedAccessException("User not authenticated.");

        var forEmployeeId = request.ForEmployeeId ?? currentEmployeeId;

        var approverPositionId = await _approverService.GetApproverPositionIdAsync(forEmployeeId, cancellationToken);

        if (!approverPositionId.HasValue)
        {
            throw new InvalidOperationException("Cannot determine approver. Employee may not have a manager.");
        }

        var remoteWorkRequest = new RemoteWorkRequest
        {
            RequesterId = currentEmployeeId,
            ForEmployeeId = forEmployeeId,
            Status = RequestStatus.Submitted,
            ApproverPositionId = approverPositionId.Value,
            SubmittedDate = DateTime.UtcNow,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Location = request.Location,
            Reason = request.Reason
        };

        _context.RemoteWorkRequests.Add(remoteWorkRequest);
        await _context.SaveChangesAsync(cancellationToken);

        // Reload with related entities
        var createdRequest = await _context.RemoteWorkRequests
            .Include(r => r.Requester)
            .Include(r => r.ForEmployee)
            .Include(r => r.ApproverPosition)
            .FirstAsync(r => r.Id == remoteWorkRequest.Id, cancellationToken);

        return new RemoteWorkRequestDto
        {
            Id = createdRequest.Id,
            RequestType = createdRequest.RequestType.ToString(),
            RequesterId = createdRequest.RequesterId,
            RequesterName = $"{createdRequest.Requester.FirstName} {createdRequest.Requester.LastName}",
            ForEmployeeId = createdRequest.ForEmployeeId,
            ForEmployeeName = $"{createdRequest.ForEmployee.FirstName} {createdRequest.ForEmployee.LastName}",
            Status = createdRequest.Status.ToString(),
            ApproverPositionId = createdRequest.ApproverPositionId,
            ApproverPositionName = createdRequest.ApproverPosition.Name,
            ApproverId = createdRequest.ApproverId,
            ApproverName = createdRequest.Approver != null ? $"{createdRequest.Approver.FirstName} {createdRequest.Approver.LastName}" : null,
            ApprovalDate = createdRequest.ApprovalDate,
            RejectionReason = createdRequest.RejectionReason,
            SubmittedDate = createdRequest.SubmittedDate,
            StartDate = createdRequest.StartDate,
            EndDate = createdRequest.EndDate,
            Location = createdRequest.Location,
            Reason = createdRequest.Reason
        };
    }
}
