using HRSystem.Application.Common.Interfaces;
using HRSystem.Application.Requests.LeaveRequests.Common;
using HRSystem.Domain.Entities;
using HRSystem.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRSystem.Application.Requests.LeaveRequests.Commands.CreateLeaveRequest;

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

        // Determine who the request is for
        var forEmployeeId = request.ForEmployeeId ?? currentEmployeeId;

        // Get approver position for the target employee
        var approverPositionId = await _approverService.GetApproverPositionIdAsync(forEmployeeId, cancellationToken);

        if (!approverPositionId.HasValue)
        {
            throw new InvalidOperationException("Cannot determine approver. Employee may not have a manager.");
        }

        // Calculate total days
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

        // Load related entities for response
        await _context.Entry(leaveRequest).Reference(r => r.Requester).LoadAsync(cancellationToken);
        await _context.Entry(leaveRequest).Reference(r => r.ForEmployee).LoadAsync(cancellationToken);
        await _context.Entry(leaveRequest).Reference(r => r.ApproverPosition).LoadAsync(cancellationToken);

        return MapToDto(leaveRequest);
    }

    private LeaveRequestDto MapToDto(LeaveRequest request)
    {
        return new LeaveRequestDto
        {
            Id = request.Id,
            RequestType = request.RequestType.ToString(),
            RequesterId = request.RequesterId,
            RequesterName = $"{request.Requester.FirstName} {request.Requester.LastName}",
            ForEmployeeId = request.ForEmployeeId,
            ForEmployeeName = $"{request.ForEmployee.FirstName} {request.ForEmployee.LastName}",
            Status = request.Status.ToString(),
            ApproverPositionId = request.ApproverPositionId,
            ApproverPositionName = request.ApproverPosition.Name,
            ApproverId = request.ApproverId,
            ApproverName = request.Approver != null ? $"{request.Approver.FirstName} {request.Approver.LastName}" : null,
            ApprovalDate = request.ApprovalDate,
            RejectionReason = request.RejectionReason,
            SubmittedDate = request.SubmittedDate,
            LeaveType = request.LeaveType,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            TotalDays = request.TotalDays,
            Reason = request.Reason
        };
    }
}
