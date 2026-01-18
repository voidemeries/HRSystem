using HRSystem.Application.Common.Interfaces;
using HRSystem.Application.Requests.ExpenseRequests.Common;
using HRSystem.Domain.Entities;
using HRSystem.Domain.Enums;
using MediatR;

namespace HRSystem.Application.Requests.ExpenseRequests.Commands.CreateExpenseRequest;

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

        await _context.Entry(expenseRequest).Reference(r => r.Requester).LoadAsync(cancellationToken);
        await _context.Entry(expenseRequest).Reference(r => r.ForEmployee).LoadAsync(cancellationToken);
        await _context.Entry(expenseRequest).Reference(r => r.ApproverPosition).LoadAsync(cancellationToken);

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
            SubmittedDate = expenseRequest.SubmittedDate,
            ExpenseDate = expenseRequest.ExpenseDate,
            Category = expenseRequest.Category,
            Amount = expenseRequest.Amount,
            Description = expenseRequest.Description,
            ReceiptAttached = expenseRequest.ReceiptAttached
        };
    }
}