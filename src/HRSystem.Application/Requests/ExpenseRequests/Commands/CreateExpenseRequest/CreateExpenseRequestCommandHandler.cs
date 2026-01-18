namespace HRSystem.Application.Requests.ExpenseRequests.Commands.CreateExpenseRequest;

using HRSystem.Application.Common.Interfaces;
using HRSystem.Application.Requests.ExpenseRequests.Common;
using HRSystem.Domain.Entities;
using HRSystem.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class CreateExpenseRequestCommandHandler : IRequestHandler<CreateExpenseRequestCommand, ExpenseRequestDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IApproverService _approverService;

    public CreateExpenseRequestCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        IApproverService approverService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _approverService = approverService;
    }

    public async Task<ExpenseRequestDto> Handle(CreateExpenseRequestCommand request, CancellationToken cancellationToken)
    {
        var currentEmployeeId = _currentUserService.EmployeeId
            ?? throw new UnauthorizedAccessException("User not authenticated.");

        var forEmployeeId = request.ForEmployeeId ?? currentEmployeeId;

        var approverPositionId = await _approverService.GetApproverPositionIdAsync(forEmployeeId, cancellationToken);

        if (!approverPositionId.HasValue)
        {
            throw new InvalidOperationException("Cannot determine approver. Employee may not have a manager.");
        }

        var expenseRequest = new ExpenseRequest
        {
            RequesterId = currentEmployeeId,
            ForEmployeeId = forEmployeeId,
            Status = RequestStatus.Submitted,
            ApproverPositionId = approverPositionId.Value,
            SubmittedDate = DateTime.UtcNow,
            ExpenseDate = request.ExpenseDate,
            Category = request.Category,
            Amount = request.Amount,
            Description = request.Description,
            ReceiptAttached = request.ReceiptAttached
        };

        _context.ExpenseRequests.Add(expenseRequest);
        await _context.SaveChangesAsync(cancellationToken);

        // Reload with related entities
        var createdRequest = await _context.ExpenseRequests
            .Include(r => r.Requester)
            .Include(r => r.ForEmployee)
            .Include(r => r.ApproverPosition)
            .FirstAsync(r => r.Id == expenseRequest.Id, cancellationToken);

        return new ExpenseRequestDto
        {
            Id = createdRequest.Id,
            RequestType = createdRequest.RequestType.ToString(),
            RequesterId = createdRequest.RequesterId,
            RequesterName = $"{createdRequest.Requester.FirstName} {createdRequest.Requester.LastName}",
            ForEmployeeId = createdRequest.ForEmployeeId,
            ForEmployeeName = $"{createdRequest.ForEmployee.FirstName} {createdRequest.ForEmployee.LastName}",
            Status = createdRequest.Status.ToString(),
            ApproverPositionId = createdRequest.ApproverPositionId,
            ApproverPositionName = createdRequest.ApproverPosition.Name,
            ApproverId = createdRequest.ApproverId,
            ApproverName = createdRequest.Approver != null ? $"{createdRequest.Approver.FirstName} {createdRequest.Approver.LastName}" : null,
            ApprovalDate = createdRequest.ApprovalDate,
            RejectionReason = createdRequest.RejectionReason,
            SubmittedDate = createdRequest.SubmittedDate,
            ExpenseDate = createdRequest.ExpenseDate,
            Category = createdRequest.Category,
            Amount = createdRequest.Amount,
            Description = createdRequest.Description,
            ReceiptAttached = createdRequest.ReceiptAttached
        };
    }
}