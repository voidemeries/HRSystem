namespace HRSystem.Application.Requests.ExpenseRequests.Commands.RejectExpenseRequest;

using HRSystem.Application.Common.Interfaces;
using HRSystem.Application.Requests.ExpenseRequests.Common;
using HRSystem.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class RejectExpenseRequestCommandHandler : IRequestHandler<RejectExpenseRequestCommand, ExpenseRequestDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IApproverService _approverService;

    public RejectExpenseRequestCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        IApproverService approverService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _approverService = approverService;
    }

    public async Task<ExpenseRequestDto> Handle(RejectExpenseRequestCommand request, CancellationToken cancellationToken)
    {
        var currentEmployeeId = _currentUserService.EmployeeId
            ?? throw new UnauthorizedAccessException("User not authenticated.");

        var expenseRequest = await _context.ExpenseRequests
            .Include(r => r.Requester)
            .Include(r => r.ForEmployee)
            .Include(r => r.ApproverPosition)
            .Include(r => r.Approver)
            .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Expense request with ID {request.Id} not found.");

        if (expenseRequest.Status != RequestStatus.Submitted)
        {
            throw new InvalidOperationException("Only submitted requests can be rejected.");
        }

        var designatedApproverId = await _approverService.GetDesignatedApproverIdAsync(
            expenseRequest.ApproverPositionId,
            cancellationToken);

        var isAuthorized = _currentUserService.IsAdmin || currentEmployeeId == designatedApproverId;

        if (!isAuthorized)
        {
            throw new UnauthorizedAccessException("You are not authorized to reject this request.");
        }

        expenseRequest.Status = RequestStatus.Rejected;
        expenseRequest.ApproverId = currentEmployeeId;
        expenseRequest.ApprovalDate = DateTime.UtcNow;
        expenseRequest.RejectionReason = request.RejectionReason;

        await _context.SaveChangesAsync(cancellationToken);

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