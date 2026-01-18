namespace HRSystem.Application.Requests.TrainingSupportRequests.Queries.GetTrainingSupportRequestById;

using HRSystem.Application.Common.Interfaces;
using HRSystem.Application.Requests.TrainingSupportRequests.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class GetTrainingSupportRequestByIdQueryHandler : IRequestHandler<GetTrainingSupportRequestByIdQuery, TrainingSupportRequestDto>
{
    private readonly IApplicationDbContext _context;

    public GetTrainingSupportRequestByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<TrainingSupportRequestDto> Handle(GetTrainingSupportRequestByIdQuery request, CancellationToken cancellationToken)
    {
        var trainingRequest = await _context.TrainingSupportRequests
            .Include(r => r.Requester)
            .Include(r => r.ForEmployee)
            .Include(r => r.ApproverPosition)
            .Include(r => r.Approver)
            .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Training support request with ID {request.Id} not found.");

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