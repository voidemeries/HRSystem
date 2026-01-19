namespace HRSystem.Application.Requests.OvertimeRequests.Commands.RejectOvertimeRequest;

using HRSystem.Application.Common.Interfaces;
using HRSystem.Application.Requests.OvertimeRequests.Common;
using HRSystem.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class RejectOvertimeRequestCommandHandler : IRequestHandler<RejectOvertimeRequestCommand, OvertimeRequestDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IApproverService _approverService;

    public RejectOvertimeRequestCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        IApproverService approverService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _approverService = approverService;
    }

    public async Task<OvertimeRequestDto> Handle(RejectOvertimeRequestCommand request, CancellationToken cancellationToken)
    {
        var currentEmployeeId = _currentUserService.EmployeeId
            ?? throw new UnauthorizedAccessException("User not authenticated.");

        var overtimeRequest = await _context.OvertimeRequests
            .Include(r => r.Requester)
            .Include(r => r.ForEmployee)
            .Include(r => r.ApproverPosition)
            .Include(r => r.Approver)
            .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Overtime request with ID {request.Id} not found.");

        if (overtimeRequest.Status != RequestStatus.Submitted)
        {
            throw new InvalidOperationException("Only submitted requests can be rejected.");
        }

        var designatedApproverId = await _approverService.GetDesignatedApproverIdAsync(
            overtimeRequest.ApproverPositionId,
            cancellationToken);

        var isAuthorized = _currentUserService.IsAdmin || currentEmployeeId == designatedApproverId;

        if (!isAuthorized)
        {
            throw new UnauthorizedAccessException("You are not authorized to reject this request.");
        }

        overtimeRequest.Status = RequestStatus.Rejected;
        overtimeRequest.ApproverId = currentEmployeeId;
        overtimeRequest.ApprovalDate = DateTime.UtcNow;
        overtimeRequest.RejectionReason = request.RejectionReason;

        await _context.SaveChangesAsync(cancellationToken);

        return new OvertimeRequestDto
        {
            Id = overtimeRequest.Id,
            RequestType = overtimeRequest.RequestType.ToString(),
            RequesterId = overtimeRequest.RequesterId,
            RequesterName = $"{overtimeRequest.Requester.FirstName} {overtimeRequest.Requester.LastName}",
            ForEmployeeId = overtimeRequest.ForEmployeeId,
            ForEmployeeName = $"{overtimeRequest.ForEmployee.FirstName} {overtimeRequest.ForEmployee.LastName}",
            Status = overtimeRequest.Status.ToString(),
            ApproverPositionId = overtimeRequest.ApproverPositionId,
            ApproverPositionName = overtimeRequest.ApproverPosition.Name,
            ApproverId = overtimeRequest.ApproverId,
            ApproverName = overtimeRequest.Approver != null ? $"{overtimeRequest.Approver.FirstName} {overtimeRequest.Approver.LastName}" : null,
            ApprovalDate = overtimeRequest.ApprovalDate,
            RejectionReason = overtimeRequest.RejectionReason,
            SubmittedDate = overtimeRequest.SubmittedDate,
            OvertimeDate = overtimeRequest.OvertimeDate,
            Hours = overtimeRequest.Hours,
            Reason = overtimeRequest.Reason
        };
    }
}