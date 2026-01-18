using HRSystem.Application.Common.Interfaces;
using HRSystem.Application.Requests.ExpenseRequests.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRSystem.Application.Requests.ExpenseRequests.Queries.GetExpenseRequests;

public class GetExpenseRequestsQueryHandler : IRequestHandler<GetExpenseRequestsQuery, List<ExpenseRequestDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetExpenseRequestsQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<List<ExpenseRequestDto>> Handle(GetExpenseRequestsQuery request, CancellationToken cancellationToken)
    {
        var currentEmployeeId = _currentUserService.EmployeeId
            ?? throw new UnauthorizedAccessException("User not authenticated.");

        var query = _context.ExpenseRequests
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

        return results.Select(r => new ExpenseRequestDto
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
            ExpenseDate = r.ExpenseDate,
            Category = r.Category,
            Amount = r.Amount,
            Description = r.Description,
            ReceiptAttached = r.ReceiptAttached
        }).ToList();
    }
}