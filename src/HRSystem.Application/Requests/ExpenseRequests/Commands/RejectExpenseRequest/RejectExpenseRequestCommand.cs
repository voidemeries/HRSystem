namespace HRSystem.Application.Requests.ExpenseRequests.Commands.RejectExpenseRequest;

using HRSystem.Application.Requests.ExpenseRequests.Common;
using MediatR;

public record RejectExpenseRequestCommand : IRequest<ExpenseRequestDto>
{
    public int Id { get; init; }
    public string RejectionReason { get; init; } = string.Empty;
}