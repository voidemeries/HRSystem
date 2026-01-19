namespace HRSystem.Application.Requests.TravelRequests.Commands.RejectTravelRequest;

using HRSystem.Application.Common.Interfaces;
using HRSystem.Application.Requests.TravelRequests.Common;
using HRSystem.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class RejectTravelRequestCommandHandler : IRequestHandler<RejectTravelRequestCommand, TravelRequestDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IApproverService _approverService;

    public RejectTravelRequestCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        IApproverService approverService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _approverService = approverService;
    }

    public async Task<TravelRequestDto> Handle(RejectTravelRequestCommand request, CancellationToken cancellationToken)
    {
        var currentEmployeeId = _currentUserService.EmployeeId
            ?? throw new UnauthorizedAccessException("User not authenticated.");

        var travelRequest = await _context.TravelRequests
            .Include(r => r.Requester)
            .Include(r => r.ForEmployee)
            .Include(r => r.ApproverPosition)
            .Include(r => r.Approver)
            .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Travel request with ID {request.Id} not found.");

        if (travelRequest.Status != RequestStatus.Submitted)
        {
            throw new InvalidOperationException("Only submitted requests can be rejected.");
        }

        var designatedApproverId = await _approverService.GetDesignatedApproverIdAsync(
            travelRequest.ApproverPositionId,
            cancellationToken);

        var isAuthorized = _currentUserService.IsAdmin || currentEmployeeId == designatedApproverId;

        if (!isAuthorized)
        {
            throw new UnauthorizedAccessException("You are not authorized to reject this request.");
        }

        travelRequest.Status = RequestStatus.Rejected;
        travelRequest.ApproverId = currentEmployeeId;
        travelRequest.ApprovalDate = DateTime.UtcNow;
        travelRequest.RejectionReason = request.RejectionReason;

        await _context.SaveChangesAsync(cancellationToken);

        return new TravelRequestDto
        {
            Id = travelRequest.Id,
            RequestType = travelRequest.RequestType.ToString(),
            RequesterId = travelRequest.RequesterId,
            RequesterName = $"{travelRequest.Requester.FirstName} {travelRequest.Requester.LastName}",
            ForEmployeeId = travelRequest.ForEmployeeId,
            ForEmployeeName = $"{travelRequest.ForEmployee.FirstName} {travelRequest.ForEmployee.LastName}",
            Status = travelRequest.Status.ToString(),
            ApproverPositionId = travelRequest.ApproverPositionId,
            ApproverPositionName = travelRequest.ApproverPosition.Name,
            ApproverId = travelRequest.ApproverId,
            ApproverName = travelRequest.Approver != null ? $"{travelRequest.Approver.FirstName} {travelRequest.Approver.LastName}" : null,
            ApprovalDate = travelRequest.ApprovalDate,
            RejectionReason = travelRequest.RejectionReason,
            SubmittedDate = travelRequest.SubmittedDate,
            Destination = travelRequest.Destination,
            StartDate = travelRequest.StartDate,
            EndDate = travelRequest.EndDate,
            Purpose = travelRequest.Purpose,
            EstimatedCost = travelRequest.EstimatedCost
        };
    }
}