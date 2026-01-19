namespace HRSystem.Application.Requests.ExpenseRequests.Commands.ApproveExpenseRequest;

using HRSystem.Application.Requests.ExpenseRequests.Common;
using MediatR;

public record ApproveExpenseRequestCommand : IRequest<ExpenseRequestDto>
{
    public int Id { get; init; }
}