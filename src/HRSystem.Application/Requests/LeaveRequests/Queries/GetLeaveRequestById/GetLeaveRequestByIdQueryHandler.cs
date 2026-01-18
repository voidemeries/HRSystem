namespace HRSystem.Application.Requests.LeaveRequests.Queries.GetLeaveRequestById;

using HRSystem.Application.Common.Interfaces;
using HRSystem.Application.Requests.LeaveRequests.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class GetLeaveRequestByIdQueryHandler : IRequestHandler<GetLeaveRequestByIdQuery, LeaveRequestDto>
{
    private readonly IApplicationDbContext _context;

    public GetLeaveRequestByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<LeaveRequestDto> Handle(GetLeaveRequestByIdQuery request, CancellationToken cancellationToken)
    {
        var leaveRequest = await _context.LeaveRequests
            .Include(r => r.Requester)
            .Include(r => r.ForEmployee)
            .Include(r => r.ApproverPosition)
            .Include(r => r.Approver)
            .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Leave request with ID {request.Id} not found.");

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