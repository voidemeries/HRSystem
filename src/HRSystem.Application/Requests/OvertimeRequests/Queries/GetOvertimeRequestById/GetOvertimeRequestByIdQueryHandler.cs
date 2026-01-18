using HRSystem.Application.Common.Interfaces;
using HRSystem.Application.Requests.OvertimeRequests.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRSystem.Application.Requests.OvertimeRequests.Queries.GetOvertimeRequestById;

public class GetOvertimeRequestByIdQueryHandler : IRequestHandler<GetOvertimeRequestByIdQuery, OvertimeRequestDto>
{
    private readonly IApplicationDbContext _context;

    public GetOvertimeRequestByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<OvertimeRequestDto> Handle(GetOvertimeRequestByIdQuery request, CancellationToken cancellationToken)
    {
        var overtimeRequest = await _context.OvertimeRequests
            .Include(r => r.Requester)
            .Include(r => r.ForEmployee)
            .Include(r => r.ApproverPosition)
            .Include(r => r.Approver)
            .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Overtime request with ID {request.Id} not found.");

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