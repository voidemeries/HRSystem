using HRSystem.Application.Common.Interfaces;
using HRSystem.Application.Requests.TravelReques.Common;
using HRSystem.Domain.Entities;
using HRSystem.Domain.Enums;
using MediatR;

namespace HRSystem.Application.Requests.TravelRequests.Commands.CreateTravelRequest;

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

        await _context.Entry(travelRequest).Reference(r => r.Requester).LoadAsync(cancellationToken);
        await _context.Entry(travelRequest).Reference(r => r.ForEmployee).LoadAsync(cancellationToken);
        await _context.Entry(travelRequest).Reference(r => r.ApproverPosition).LoadAsync(cancellationToken);

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
            SubmittedDate = travelRequest.SubmittedDate,
            Destination = travelRequest.Destination,
            StartDate = travelRequest.StartDate,
            EndDate = travelRequest.EndDate,
            Purpose = travelRequest.Purpose,
            EstimatedCost = travelRequest.EstimatedCost
        };
    }
}