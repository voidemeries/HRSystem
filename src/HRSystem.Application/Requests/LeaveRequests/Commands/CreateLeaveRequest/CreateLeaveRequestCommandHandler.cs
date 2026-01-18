namespace HRSystem.Application.Requests.LeaveRequests.Commands.CreateLeaveRequest;

using HRSystem.Application.Common.Interfaces;
using HRSystem.Application.Requests.LeaveRequests.Common;
using HRSystem.Domain.Entities;
using HRSystem.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class CreateLeaveRequestCommandHandler : IRequestHandler<CreateLeaveRequestCommand, LeaveRequestDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IApproverService _approverService;

    public CreateLeaveRequestCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        IApproverService approverService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _approverService = approverService;
    }

    public async Task<LeaveRequestDto> Handle(CreateLeaveRequestCommand request, CancellationToken cancellationToken)
    {
        var currentEmployeeId = _currentUserService.EmployeeId
            ?? throw new UnauthorizedAccessException("User not authenticated.");

        var forEmployeeId = request.ForEmployeeId ?? currentEmployeeId;

        var approverPositionId = await _approverService.GetApproverPositionIdAsync(forEmployeeId, cancellationToken);

        if (!approverPositionId.HasValue)
        {
            throw new InvalidOperationException("Cannot determine approver. Employee may not have a manager.");
        }

        var totalDays = (request.EndDate.Date - request.StartDate.Date).Days + 1;

        var leaveRequest = new LeaveRequest
        {
            RequesterId = currentEmployeeId,
            ForEmployeeId = forEmployeeId,
            Status = RequestStatus.Submitted,
            ApproverPositionId = approverPositionId.Value,
            SubmittedDate = DateTime.UtcNow,
            LeaveType = request.LeaveType,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            TotalDays = totalDays,
            Reason = request.Reason
        };

        _context.LeaveRequests.Add(leaveRequest);
        await _context.SaveChangesAsync(cancellationToken);

        // Reload with related entities
        var createdRequest = await _context.LeaveRequests
            .Include(r => r.Requester)
            .Include(r => r.ForEmployee)
            .Include(r => r.ApproverPosition)
            .FirstAsync(r => r.Id == leaveRequest.Id, cancellationToken);

        return new LeaveRequestDto
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
            LeaveType = createdRequest.LeaveType,
            StartDate = createdRequest.StartDate,
            EndDate = createdRequest.EndDate,
            TotalDays = createdRequest.TotalDays,
            Reason = createdRequest.Reason
        };
    }
}