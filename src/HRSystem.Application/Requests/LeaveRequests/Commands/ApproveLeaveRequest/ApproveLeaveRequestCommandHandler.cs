namespace HRSystem.Application.Requests.LeaveRequests.Commands.ApproveLeaveRequest;

using HRSystem.Application.Common.Interfaces;
using HRSystem.Application.Requests.LeaveRequests.Common;
using HRSystem.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class ApproveLeaveRequestCommandHandler : IRequestHandler<ApproveLeaveRequestCommand, LeaveRequestDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IApproverService _approverService;

    public ApproveLeaveRequestCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        IApproverService approverService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _approverService = approverService;
    }

    public async Task<LeaveRequestDto> Handle(ApproveLeaveRequestCommand request, CancellationToken cancellationToken)
    {
        var currentEmployeeId = _currentUserService.EmployeeId
            ?? throw new UnauthorizedAccessException("User not authenticated.");

        var leaveRequest = await _context.LeaveRequests
            .Include(r => r.Requester)
            .Include(r => r.ForEmployee)
            .Include(r => r.ApproverPosition)
            .Include(r => r.Approver)
            .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Leave request with ID {request.Id} not found.");

        if (leaveRequest.Status != RequestStatus.Submitted)
        {
            throw new InvalidOperationException("Only submitted requests can be approved.");
        }

        // Determine designated approver for the approver position
        var designatedApproverId = await _approverService.GetDesignatedApproverIdAsync(
            leaveRequest.ApproverPositionId,
            cancellationToken);

        // Check authorization
        var isAuthorized = _currentUserService.IsAdmin || currentEmployeeId == designatedApproverId;

        if (!isAuthorized)
        {
            throw new UnauthorizedAccessException("You are not authorized to approve this request.");
        }

        leaveRequest.Status = RequestStatus.Approved;
        leaveRequest.ApproverId = currentEmployeeId;
        leaveRequest.ApprovalDate = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return new LeaveRequestDto
        {
            Id = leaveRequest.Id,
            RequestType = leaveRequest.RequestType.ToString(),
            RequesterId = leaveRequest.RequesterId,
            RequesterName = $"{leaveRequest.Requester.FirstName} {leaveRequest.Requester.LastName}",
            ForEmployeeId = leaveRequest.ForEmployeeId,
            ForEmployeeName = $"{leaveRequest.ForEmployee.FirstName} {leaveRequest.ForEmployee.LastName}",
            Status = leaveRequest.Status.ToString(),
            ApproverPositionId = leaveRequest.ApproverPositionId,
            ApproverPositionName = leaveRequest.ApproverPosition.Name,
            ApproverId = leaveRequest.ApproverId,
            ApproverName = leaveRequest.Approver != null ? $"{leaveRequest.Approver.FirstName} {leaveRequest.Approver.LastName}" : null,
            ApprovalDate = leaveRequest.ApprovalDate,
            RejectionReason = leaveRequest.RejectionReason,
            SubmittedDate = leaveRequest.SubmittedDate,
            LeaveType = leaveRequest.LeaveType,
            StartDate = leaveRequest.StartDate,
            EndDate = leaveRequest.EndDate,
            TotalDays = leaveRequest.TotalDays,
            Reason = leaveRequest.Reason
        };
    }
}