namespace HRSystem.Application.Requests.TravelRequests.Queries.GetTravelRequests;

using HRSystem.Application.Common.Interfaces;
using HRSystem.Application.Requests.TravelRequests.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class GetTravelRequestsQueryHandler : IRequestHandler<GetTravelRequestsQuery, List<TravelRequestDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetTravelRequestsQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<List<TravelRequestDto>> Handle(GetTravelRequestsQuery request, CancellationToken cancellationToken)
    {
        var currentEmployeeId = _currentUserService.EmployeeId
            ?? throw new UnauthorizedAccessException("User not authenticated.");

        var query = _context.TravelRequests
            .Include(r => r.Requester)
            .Include(r => r.ForEmployee)
            .Include(r => r.ApproverPosition)
            .Include(r => r.Approver)
            .AsQueryable();

        if (request.Mine == true)
        {
            query = query.Where(r => r.ForEmployeeId == currentEmployeeId);
        }

        if (request.ForEmployeeId.HasValue)
        {
            query = query.Where(r => r.ForEmployeeId == request.ForEmployeeId.Value);
        }

        if (request.Status.HasValue)
        {
            query = query.Where(r => r.Status == request.Status.Value);
        }

        var results = await query
            .OrderByDescending(r => r.SubmittedDate)
            .ToListAsync(cancellationToken);

        return results.Select(r => new TravelRequestDto
        {
            Id = r.Id,
            RequestType = r.RequestType.ToString(),
            RequesterId = r.RequesterId,
            RequesterName = $"{r.Requester.FirstName} {r.Requester.LastName}",
            ForEmployeeId = r.ForEmployeeId,
            ForEmployeeName = $"{r.ForEmployee.FirstName} {r.ForEmployee.LastName}",
            Status = r.Status.ToString(),
            ApproverPositionId = r.ApproverPositionId,
            ApproverPositionName = r.ApproverPosition.Name,
            ApproverId = r.ApproverId,
            ApproverName = r.Approver != null ? $"{r.Approver.FirstName} {r.Approver.LastName}" : null,
            ApprovalDate = r.ApprovalDate,
            RejectionReason = r.RejectionReason,
            SubmittedDate = r.SubmittedDate,
            Destination = r.Destination,
            StartDate = r.StartDate,
            EndDate = r.EndDate,
            Purpose = r.Purpose,
            EstimatedCost = r.EstimatedCost
        }).ToList();
    }
}