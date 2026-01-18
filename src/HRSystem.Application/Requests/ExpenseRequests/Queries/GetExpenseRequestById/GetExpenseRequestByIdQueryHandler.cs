using HRSystem.Application.Common.Interfaces;
using HRSystem.Application.Requests.ExpenseRequests.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRSystem.Application.Requests.ExpenseRequests.Queries.GetExpenseRequestById;

public class GetExpenseRequestByIdQueryHandler : IRequestHandler<GetExpenseRequestByIdQuery, ExpenseRequestDto>
{
    private readonly IApplicationDbContext _context;

    public GetExpenseRequestByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ExpenseRequestDto> Handle(GetExpenseRequestByIdQuery request, CancellationToken cancellationToken)
    {
        var expenseRequest = await _context.ExpenseRequests
            .Include(r => r.Requester)
            .Include(r => r.ForEmployee)
            .Include(r => r.ApproverPosition)
            .Include(r => r.Approver)
            .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Expense request with ID {request.Id} not found.");

        return new ExpenseRequestDto
        {
            Id = expenseRequest.Id,
            RequestType = expenseRequest.RequestType.ToString(),
            RequesterId = expenseRequest.RequesterId,
            RequesterName = $"{expenseRequest.Requester.FirstName} {expenseRequest.Requester.LastName}",
            ForEmployeeId = expenseRequest.ForEmployeeId,
            ForEmployeeName = $"{expenseRequest.ForEmployee.FirstName} {expenseRequest.ForEmployee.LastName}",
            Status = expenseRequest.Status.ToString(),
            ApproverPositionId = expenseRequest.ApproverPositionId,
            ApproverPositionName = expenseRequest.ApproverPosition.Name,
            ApproverId = expenseRequest.ApproverId,
            ApproverName = expenseRequest.Approver != null ? $"{expenseRequest.Approver.FirstName} {expenseRequest.Approver.LastName}" : null,
            ApprovalDate = expenseRequest.ApprovalDate,
            RejectionReason = expenseRequest.RejectionReason,
            SubmittedDate = expenseRequest.SubmittedDate,
            ExpenseDate = expenseRequest.ExpenseDate,
            Category = expenseRequest.Category,
            Amount = expenseRequest.Amount,
            Description = expenseRequest.Description,
            ReceiptAttached = expenseRequest.ReceiptAttached
        };
    }
}