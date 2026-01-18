using HRSystem.Application.Requests.ExpenseRequests.Common;
using MediatR;

namespace HRSystem.Application.Requests.ExpenseRequests.Commands.CreateExpenseRequest;

public record CreateExpenseRequestCommand : IRequest<ExpenseRequestDto>
{
    public int? ForEmployeeId { get; init; }
    public DateTime ExpenseDate { get; init; }
    public string Category { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public string Description { get; init; } = string.Empty;
    public bool ReceiptAttached { get; init; }
}