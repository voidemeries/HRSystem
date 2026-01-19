namespace HRSystem.Application.Requests.Queries.GetPendingApprovals;

using global::HRSystem.Application.Common.Interfaces;
using global::HRSystem.Application.Requests.Common;
using global::HRSystem.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class GetPendingApprovalsQueryHandler : IRequestHandler<GetPendingApprovalsQuery, List<PendingApprovalDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IApproverService _approverService;

    public GetPendingApprovalsQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        IApproverService approverService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _approverService = approverService;
    }

    public async Task<List<PendingApprovalDto>> Handle(GetPendingApprovalsQuery request, CancellationToken cancellationToken)
    {
        var currentEmployeeId = _currentUserService.EmployeeId
            ?? throw new UnauthorizedAccessException("User not authenticated.");

        var isAdmin = _currentUserService.IsAdmin;
        var result = new List<PendingApprovalDto>();

        // Get all positions where the current employee is the designated approver
        var employee = await _context.Employees
            .Include(e => e.Position)
            .FirstOrDefaultAsync(e => e.Id == currentEmployeeId, cancellationToken);

        if (employee == null)
            return result;

        // Find all positions where this employee's position is the parent
        var approverPositionIds = await _context.Positions
            .Where(p => p.ParentPositionId == employee.PositionId)
            .Select(p => p.Id)
            .ToListAsync(cancellationToken);

        // Also include the employee's own position
        approverPositionIds.Add(employee.PositionId);

        // Get pending leave requests
        var leaveRequests = await _context.LeaveRequests
            .Include(r => r.Requester)
            .Include(r => r.ForEmployee)
            .Where(r => r.Status == RequestStatus.Submitted &&
                       (isAdmin || approverPositionIds.Contains(r.ApproverPositionId)))
            .ToListAsync(cancellationToken);

        foreach (var req in leaveRequests)
        {
            var designatedApproverId = await _approverService.GetDesignatedApproverIdAsync(
                req.ApproverPositionId, cancellationToken);

            if (isAdmin || currentEmployeeId == designatedApproverId)
            {
                result.Add(new PendingApprovalDto
                {
                    Id = req.Id,
                    RequestType = "Leave",
                    RequesterId = req.RequesterId,
                    RequesterName = $"{req.Requester.FirstName} {req.Requester.LastName}",
                    ForEmployeeId = req.ForEmployeeId,
                    ForEmployeeName = $"{req.ForEmployee.FirstName} {req.ForEmployee.LastName}",
                    SubmittedDate = req.SubmittedDate,
                    Summary = $"{req.LeaveType}: {req.StartDate:yyyy-MM-dd} to {req.EndDate:yyyy-MM-dd} ({req.TotalDays} days)"
                });
            }
        }

        // Get pending remote work requests
        var remoteWorkRequests = await _context.RemoteWorkRequests
            .Include(r => r.Requester)
            .Include(r => r.ForEmployee)
            .Where(r => r.Status == RequestStatus.Submitted &&
                       (isAdmin || approverPositionIds.Contains(r.ApproverPositionId)))
            .ToListAsync(cancellationToken);

        foreach (var req in remoteWorkRequests)
        {
            var designatedApproverId = await _approverService.GetDesignatedApproverIdAsync(
                req.ApproverPositionId, cancellationToken);

            if (isAdmin || currentEmployeeId == designatedApproverId)
            {
                result.Add(new PendingApprovalDto
                {
                    Id = req.Id,
                    RequestType = "RemoteWork",
                    RequesterId = req.RequesterId,
                    RequesterName = $"{req.Requester.FirstName} {req.Requester.LastName}",
                    ForEmployeeId = req.ForEmployeeId,
                    ForEmployeeName = $"{req.ForEmployee.FirstName} {req.ForEmployee.LastName}",
                    SubmittedDate = req.SubmittedDate,
                    Summary = $"{req.Location}: {req.StartDate:yyyy-MM-dd} to {req.EndDate:yyyy-MM-dd}"
                });
            }
        }

        // Get pending travel requests
        var travelRequests = await _context.TravelRequests
            .Include(r => r.Requester)
            .Include(r => r.ForEmployee)
            .Where(r => r.Status == RequestStatus.Submitted &&
                       (isAdmin || approverPositionIds.Contains(r.ApproverPositionId)))
            .ToListAsync(cancellationToken);

        foreach (var req in travelRequests)
        {
            var designatedApproverId = await _approverService.GetDesignatedApproverIdAsync(
                req.ApproverPositionId, cancellationToken);

            if (isAdmin || currentEmployeeId == designatedApproverId)
            {
                result.Add(new PendingApprovalDto
                {
                    Id = req.Id,
                    RequestType = "Travel",
                    RequesterId = req.RequesterId,
                    RequesterName = $"{req.Requester.FirstName} {req.Requester.LastName}",
                    ForEmployeeId = req.ForEmployeeId,
                    ForEmployeeName = $"{req.ForEmployee.FirstName} {req.ForEmployee.LastName}",
                    SubmittedDate = req.SubmittedDate,
                    Summary = $"{req.Destination}: {req.StartDate:yyyy-MM-dd} to {req.EndDate:yyyy-MM-dd} (${req.EstimatedCost:N2})"
                });
            }
        }

        // Get pending expense requests
        var expenseRequests = await _context.ExpenseRequests
            .Include(r => r.Requester)
            .Include(r => r.ForEmployee)
            .Where(r => r.Status == RequestStatus.Submitted &&
                       (isAdmin || approverPositionIds.Contains(r.ApproverPositionId)))
            .ToListAsync(cancellationToken);

        foreach (var req in expenseRequests)
        {
            var designatedApproverId = await _approverService.GetDesignatedApproverIdAsync(
                req.ApproverPositionId, cancellationToken);

            if (isAdmin || currentEmployeeId == designatedApproverId)
            {
                result.Add(new PendingApprovalDto
                {
                    Id = req.Id,
                    RequestType = "Expense",
                    RequesterId = req.RequesterId,
                    RequesterName = $"{req.Requester.FirstName} {req.Requester.LastName}",
                    ForEmployeeId = req.ForEmployeeId,
                    ForEmployeeName = $"{req.ForEmployee.FirstName} {req.ForEmployee.LastName}",
                    SubmittedDate = req.SubmittedDate,
                    Summary = $"{req.Category}: ${req.Amount:N2} on {req.ExpenseDate:yyyy-MM-dd}"
                });
            }
        }

        // Get pending overtime requests
        var overtimeRequests = await _context.OvertimeRequests
            .Include(r => r.Requester)
            .Include(r => r.ForEmployee)
            .Where(r => r.Status == RequestStatus.Submitted &&
                       (isAdmin || approverPositionIds.Contains(r.ApproverPositionId)))
            .ToListAsync(cancellationToken);

        foreach (var req in overtimeRequests)
        {
            var designatedApproverId = await _approverService.GetDesignatedApproverIdAsync(
                req.ApproverPositionId, cancellationToken);

            if (isAdmin || currentEmployeeId == designatedApproverId)
            {
                result.Add(new PendingApprovalDto
                {
                    Id = req.Id,
                    RequestType = "Overtime",
                    RequesterId = req.RequesterId,
                    RequesterName = $"{req.Requester.FirstName} {req.Requester.LastName}",
                    ForEmployeeId = req.ForEmployeeId,
                    ForEmployeeName = $"{req.ForEmployee.FirstName} {req.ForEmployee.LastName}",
                    SubmittedDate = req.SubmittedDate,
                    Summary = $"{req.Hours} hours on {req.OvertimeDate:yyyy-MM-dd}"
                });
            }
        }

        // Get pending training support requests
        var trainingRequests = await _context.TrainingSupportRequests
            .Include(r => r.Requester)
            .Include(r => r.ForEmployee)
            .Where(r => r.Status == RequestStatus.Submitted &&
                       (isAdmin || approverPositionIds.Contains(r.ApproverPositionId)))
            .ToListAsync(cancellationToken);

        foreach (var req in trainingRequests)
        {
            var designatedApproverId = await _approverService.GetDesignatedApproverIdAsync(
                req.ApproverPositionId, cancellationToken);

            if (isAdmin || currentEmployeeId == designatedApproverId)
            {
                result.Add(new PendingApprovalDto
                {
                    Id = req.Id,
                    RequestType = "TrainingSupport",
                    RequesterId = req.RequesterId,
                    RequesterName = $"{req.Requester.FirstName} {req.Requester.LastName}",
                    ForEmployeeId = req.ForEmployeeId,
                    ForEmployeeName = $"{req.ForEmployee.FirstName} {req.ForEmployee.LastName}",
                    SubmittedDate = req.SubmittedDate,
                    Summary = $"{req.TrainingName} by {req.Provider} (${req.Cost:N2})"
                });
            }
        }

        return result.OrderByDescending(r => r.SubmittedDate).ToList();
    }
}