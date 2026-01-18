using HRSystem.Application.Common.Interfaces;
using HRSystem.Application.Requests.TrainingSupportRequests.Common;
using HRSystem.Domain.Entities;
using HRSystem.Domain.Enums;
using MediatR;

namespace HRSystem.Application.Requests.TrainingSupportRequests.Commands.CreateTrainingSupportRequest;

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
            StartDate = request.StartDate,
            EndDate = request.EndDate,
        };

        _context.TrainingSupportRequests.Add(trainingSupportRequest);
        await _context.SaveChangesAsync(cancellationToken);

        await _context.Entry(trainingSupportRequest).Reference(r => r.Requester).LoadAsync(cancellationToken);
        await _context.Entry(trainingSupportRequest).Reference(r => r.ForEmployee).LoadAsync(cancellationToken);
        await _context.Entry(trainingSupportRequest).Reference(r => r.ApproverPosition).LoadAsync(cancellationToken);

        return new TrainingSupportRequestDto
        {
            Id = trainingSupportRequest.Id,
            RequestType = trainingSupportRequest.RequestType.ToString(),
            RequesterId = trainingSupportRequest.RequesterId,
            RequesterName = $"{trainingSupportRequest.Requester.FirstName} {trainingSupportRequest.Requester.LastName}",
            ForEmployeeId = trainingSupportRequest.ForEmployeeId,
            ForEmployeeName = $"{trainingSupportRequest.ForEmployee.FirstName} {trainingSupportRequest.ForEmployee.LastName}",
            Status = trainingSupportRequest.Status.ToString(),
            ApproverPositionId = trainingSupportRequest.ApproverPositionId,
            ApproverPositionName = trainingSupportRequest.ApproverPosition.Name,
            SubmittedDate = trainingSupportRequest.SubmittedDate,
            StartDate = trainingSupportRequest.StartDate,
            EndDate = trainingSupportRequest.EndDate
        };
    }
}