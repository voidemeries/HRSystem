using HRSystem.Application.Common.Interfaces;
using HRSystem.Application.Requests.OvertimeRequests.Common;
using HRSystem.Domain.Entities;
using HRSystem.Domain.Enums;
using MediatR;

namespace HRSystem.Application.Requests.OvertimeRequests.Commands.CreateOvertimeRequest;

public class CreateOvertimeRequestCommandHandler : IRequestHandler<CreateOvertimeRequestCommand, OvertimeRequestDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IApproverService _approverService;

    public CreateOvertimeRequestCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        IApproverService approverService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _approverService = approverService;
    }

    public async Task<OvertimeRequestDto> Handle(CreateOvertimeRequestCommand request, CancellationToken cancellationToken)
    {
        var currentEmployeeId = _currentUserService.EmployeeId
            ?? throw new UnauthorizedAccessException("User not authenticated.");

        var forEmployeeId = request.ForEmployeeId ?? currentEmployeeId;

        var approverPositionId = await _approverService.GetApproverPositionIdAsync(forEmployeeId, cancellationToken);

        if (!approverPositionId.HasValue)
        {
            throw new InvalidOperationException("Cannot determine approver. Employee may not have a manager.");
        }

        var overtimeRequest = new OvertimeRequest
        {
            RequesterId = currentEmployeeId,
            ForEmployeeId = forEmployeeId,
            Status = RequestStatus.Submitted,
            ApproverPositionId = approverPositionId.Value,
            SubmittedDate = DateTime.UtcNow,
            OvertimeDate = request.OvertimeDate,
            Hours = request.Hours,
            Reason = request.Reason
        };

        _context.OvertimeRequests.Add(overtimeRequest);
        await _context.SaveChangesAsync(cancellationToken);

        await _context.Entry(overtimeRequest).Reference(r => r.Requester).LoadAsync(cancellationToken);
        await _context.Entry(overtimeRequest).Reference(r => r.ForEmployee).LoadAsync(cancellationToken);
        await _context.Entry(overtimeRequest).Reference(r => r.ApproverPosition).LoadAsync(cancellationToken);

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
            SubmittedDate = overtimeRequest.SubmittedDate,
            OvertimeDate = overtimeRequest.OvertimeDate,
            Hours = overtimeRequest.Hours,
            Reason = overtimeRequest.Reason
        };
    }
}