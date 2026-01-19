namespace HRSystem.Application.Requests.Queries.GetMyApprovals;

using HRSystem.Application.Common.Interfaces;
using HRSystem.Application.Requests.Common;
using HRSystem.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class GetMyApprovalsQueryHandler : IRequestHandler<GetMyApprovalsQuery, List<PendingApprovalDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetMyApprovalsQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<List<PendingApprovalDto>> Handle(GetMyApprovalsQuery request, CancellationToken cancellationToken)
    {
        var currentEmployeeId = _currentUserService.EmployeeId
            ?? throw new UnauthorizedAccessException("User not authenticated.");

        var result = new List<PendingApprovalDto>();

        // Build query filter
        var statusFilter = request.Status ?? RequestStatus.Submitted;

        // Get leave requests approved/rejected by current user
        var leaveRequests = await _context.LeaveRequests
            .Include(r => r.Requester)
            .Include(r => r.ForEmployee)
            .Where(r => r.ApproverId == currentEmployeeId && r.Status == statusFilter)
            .ToListAsync(cancellationToken);

        result.AddRange(leaveRequests.Select(req => new PendingApprovalDto
        {
            Id = req.Id,
            RequestType = "Leave",
            RequesterId = req.RequesterId,
            RequesterName = $"{req.Requester.FirstName} {req.Requester.LastName}",
            ForEmployeeId = req.ForEmployeeId,
            ForEmployeeName = $"{req.ForEmployee.FirstName} {req.ForEmployee.LastName}",
            SubmittedDate = req.SubmittedDate,
            Summary = $"{req.LeaveType}: {req.StartDate:yyyy-MM-dd} to {req.EndDate:yyyy-MM-dd} ({req.TotalDays} days)"
        }));

        // Similar for other request types...
        var remoteWorkRequests = await _context.RemoteWorkRequests
            .Include(r => r.Requester)
            .Include(r => r.ForEmployee)
            .Where(r => r.ApproverId == currentEmployeeId && r.Status == statusFilter)
            .ToListAsync(cancellationToken);

        result.AddRange(remoteWorkRequests.Select(req => new PendingApprovalDto
        {
            Id = req.Id,
            RequestType = "RemoteWork",
            RequesterId = req.RequesterId,
            RequesterName = $"{req.Requester.FirstName} {req.Requester.LastName}",
            ForEmployeeId = req.ForEmployeeId,
            ForEmployeeName = $"{req.ForEmployee.FirstName} {req.ForEmployee.LastName}",
            SubmittedDate = req.SubmittedDate,
            Summary = $"{req.Location}: {req.StartDate:yyyy-MM-dd} to {req.EndDate:yyyy-MM-dd}"
        }));

        return result.OrderByDescending(r => r.SubmittedDate).ToList();
    }
}