namespace HRSystem.Application.Requests.TravelRequests.Commands.CreateTravelRequest;

using HRSystem.Application.Common.Interfaces;
using HRSystem.Application.Requests.TravelRequests.Common;
using HRSystem.Domain.Entities;
using HRSystem.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class CreateTravelRequestCommandHandler : IRequestHandler<CreateTravelRequestCommand, TravelRequestDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IApproverService _approverService;

    public CreateTravelRequestCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        IApproverService approverService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _approverService = approverService;
    }

    public async Task<TravelRequestDto> Handle(CreateTravelRequestCommand request, CancellationToken cancellationToken)
    {
        var currentEmployeeId = _currentUserService.EmployeeId
            ?? throw new UnauthorizedAccessException("User not authenticated.");

        var forEmployeeId = request.ForEmployeeId ?? currentEmployeeId;

        var approverPositionId = await _approverService.GetApproverPositionIdAsync(forEmployeeId, cancellationToken);

        if (!approverPositionId.HasValue)
        {
            throw new InvalidOperationException("Cannot determine approver. Employee may not have a manager.");
        }

        var travelRequest = new TravelRequest
        {
            RequesterId = currentEmployeeId,
            ForEmployeeId = forEmployeeId,
            Status = RequestStatus.Submitted,
            ApproverPositionId = approverPositionId.Value,
            SubmittedDate = DateTime.UtcNow,
            Destination = request.Destination,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Purpose = request.Purpose,
            EstimatedCost = request.EstimatedCost
        };

        _context.TravelRequests.Add(travelRequest);
        await _context.SaveChangesAsync(cancellationToken);

        // Reload with related entities
        var createdRequest = await _context.TravelRequests
            .Include(r => r.Requester)
            .Include(r => r.ForEmployee)
            .Include(r => r.ApproverPosition)
            .FirstAsync(r => r.Id == travelRequest.Id, cancellationToken);

        return new TravelRequestDto
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
            Destination = createdRequest.Destination,
            StartDate = createdRequest.StartDate,
            EndDate = createdRequest.EndDate,
            Purpose = createdRequest.Purpose,
            EstimatedCost = createdRequest.EstimatedCost
        };
    }
}