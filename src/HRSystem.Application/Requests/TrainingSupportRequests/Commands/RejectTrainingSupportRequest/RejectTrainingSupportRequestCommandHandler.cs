namespace HRSystem.Application.Requests.TrainingSupportRequests.Commands.RejectTrainingSupportRequest;

using HRSystem.Application.Common.Interfaces;
using HRSystem.Application.Requests.TrainingSupportRequests.Common;
using HRSystem.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class RejectTrainingSupportRequestCommandHandler : IRequestHandler<RejectTrainingSupportRequestCommand, TrainingSupportRequestDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IApproverService _approverService;

    public RejectTrainingSupportRequestCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        IApproverService approverService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _approverService = approverService;
    }

    public async Task<TrainingSupportRequestDto> Handle(RejectTrainingSupportRequestCommand request, CancellationToken cancellationToken)
    {
        var currentEmployeeId = _currentUserService.EmployeeId
            ?? throw new UnauthorizedAccessException("User not authenticated.");

        var trainingRequest = await _context.TrainingSupportRequests
            .Include(r => r.Requester)
            .Include(r => r.ForEmployee)
            .Include(r => r.ApproverPosition)
            .Include(r => r.Approver)
            .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Training support request with ID {request.Id} not found.");

        if (trainingRequest.Status != RequestStatus.Submitted)
        {
            throw new InvalidOperationException("Only submitted requests can be rejected.");
        }

        var designatedApproverId = await _approverService.GetDesignatedApproverIdAsync(
            trainingRequest.ApproverPositionId,
            cancellationToken);

        var isAuthorized = _currentUserService.IsAdmin || currentEmployeeId == designatedApproverId;

        if (!isAuthorized)
        {
            throw new UnauthorizedAccessException("You are not authorized to reject this request.");
        }

        trainingRequest.Status = RequestStatus.Rejected;
        trainingRequest.ApproverId = currentEmployeeId;
        trainingRequest.ApprovalDate = DateTime.UtcNow;
        trainingRequest.RejectionReason = request.RejectionReason;

        await _context.SaveChangesAsync(cancellationToken);

        return new TrainingSupportRequestDto
        {
            Id = trainingRequest.Id,
            RequestType = trainingRequest.RequestType.ToString(),
            RequesterId = trainingRequest.RequesterId,
            RequesterName = $"{trainingRequest.Requester.FirstName} {trainingRequest.Requester.LastName}",
            ForEmployeeId = trainingRequest.ForEmployeeId,
            ForEmployeeName = $"{trainingRequest.ForEmployee.FirstName} {trainingRequest.ForEmployee.LastName}",
            Status = trainingRequest.Status.ToString(),
            ApproverPositionId = trainingRequest.ApproverPositionId,
            ApproverPositionName = trainingRequest.ApproverPosition.Name,
            ApproverId = trainingRequest.ApproverId,
            ApproverName = trainingRequest.Approver != null ? $"{trainingRequest.Approver.FirstName} {trainingRequest.Approver.LastName}" : null,
            ApprovalDate = trainingRequest.ApprovalDate,
            RejectionReason = trainingRequest.RejectionReason,
            SubmittedDate = trainingRequest.SubmittedDate,
            TrainingName = trainingRequest.TrainingName,
            Provider = trainingRequest.Provider,
            StartDate = trainingRequest.StartDate,
            EndDate = trainingRequest.EndDate,
            Cost = trainingRequest.Cost,
            Justification = trainingRequest.Justification
        };
    }
}