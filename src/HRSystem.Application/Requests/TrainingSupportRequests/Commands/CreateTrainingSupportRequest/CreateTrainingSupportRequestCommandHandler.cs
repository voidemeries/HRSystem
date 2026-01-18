namespace HRSystem.Application.Requests.TrainingSupportRequests.Commands.CreateTrainingSupportRequest;

using HRSystem.Application.Common.Interfaces;
using HRSystem.Application.Requests.TrainingSupportRequests.Common;
using HRSystem.Domain.Entities;
using HRSystem.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class CreateTrainingSupportRequestCommandHandler : IRequestHandler<CreateTrainingSupportRequestCommand, TrainingSupportRequestDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IApproverService _approverService;

    public CreateTrainingSupportRequestCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        IApproverService approverService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _approverService = approverService;
    }

    public async Task<TrainingSupportRequestDto> Handle(CreateTrainingSupportRequestCommand request, CancellationToken cancellationToken)
    {
        var currentEmployeeId = _currentUserService.EmployeeId
            ?? throw new UnauthorizedAccessException("User not authenticated.");

        var forEmployeeId = request.ForEmployeeId ?? currentEmployeeId;

        var approverPositionId = await _approverService.GetApproverPositionIdAsync(forEmployeeId, cancellationToken);

        if (!approverPositionId.HasValue)
        {
            throw new InvalidOperationException("Cannot determine approver. Employee may not have a manager.");
        }

        var trainingSupportRequest = new TrainingSupportRequest
        {
            RequesterId = currentEmployeeId,
            ForEmployeeId = forEmployeeId,
            Status = RequestStatus.Submitted,
            ApproverPositionId = approverPositionId.Value,
            SubmittedDate = DateTime.UtcNow,
            TrainingName = request.TrainingName,
            Provider = request.Provider,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Cost = request.Cost,
            Justification = request.Justification
        };

        _context.TrainingSupportRequests.Add(trainingSupportRequest);
        await _context.SaveChangesAsync(cancellationToken);

        // Reload with related entities
        var createdRequest = await _context.TrainingSupportRequests
            .Include(r => r.Requester)
            .Include(r => r.ForEmployee)
            .Include(r => r.ApproverPosition)
            .FirstAsync(r => r.Id == trainingSupportRequest.Id, cancellationToken);

        return new TrainingSupportRequestDto
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
            TrainingName = createdRequest.TrainingName,
            Provider = createdRequest.Provider,
            StartDate = createdRequest.StartDate,
            EndDate = createdRequest.EndDate,
            Cost = createdRequest.Cost,
            Justification = createdRequest.Justification
        };
    }
}