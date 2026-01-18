namespace HRSystem.Application.Requests.TravelRequests.Queries.GetTravelRequestById;

using HRSystem.Application.Common.Interfaces;
using HRSystem.Application.Requests.TravelRequests.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class GetTravelRequestByIdQueryHandler : IRequestHandler<GetTravelRequestByIdQuery, TravelRequestDto>
{
    private readonly IApplicationDbContext _context;

    public GetTravelRequestByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<TravelRequestDto> Handle(GetTravelRequestByIdQuery request, CancellationToken cancellationToken)
    {
        var travelRequest = await _context.TravelRequests
            .Include(r => r.Requester)
            .Include(r => r.ForEmployee)
            .Include(r => r.ApproverPosition)
            .Include(r => r.Approver)
            .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Travel request with ID {request.Id} not found.");

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