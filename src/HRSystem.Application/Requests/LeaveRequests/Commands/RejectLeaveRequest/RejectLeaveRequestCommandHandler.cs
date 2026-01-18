namespace HRSystem.Application.Requests.LeaveRequests.Commands.RejectLeaveRequest;

using HRSystem.Application.Common.Interfaces;
using HRSystem.Application.Requests.LeaveRequests.Common;
using HRSystem.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class RejectLeaveRequestCommandHandler : IRequestHandler<RejectLeaveRequestCommand, LeaveRequestDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IApproverService _approverService;

    public RejectLeaveRequestCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        IApproverService approverService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _approverService = approverService;
    }

    public async Task<LeaveRequestDto> Handle(RejectLeaveRequestCommand request, CancellationToken cancellationToken)
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
            throw new InvalidOperationException("Only submitted requests can be rejected.");
        }

        var designatedApproverId = await _approverService.GetDesignatedApproverIdAsync(
            leaveRequest.ApproverPositionId,
            cancellationToken);

        var isAuthorized = _currentUserService.IsAdmin || currentEmployeeId == designatedApproverId;

        if (!isAuthorized)
        {
            throw new UnauthorizedAccessException("You are not authorized to reject this request.");
        }

        leaveRequest.Status = RequestStatus.Rejected;
        leaveRequest.ApproverId = currentEmployeeId;
        leaveRequest.ApprovalDate = DateTime.UtcNow;
        leaveRequest.RejectionReason = request.RejectionReason;

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