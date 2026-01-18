using HRSystem.Application.Requests.ExpenseRequests.Common;
using HRSystem.Domain.Enums;
using MediatR;

namespace HRSystem.Application.Requests.ExpenseRequests.Queries.GetExpenseRequests;

public record GetExpenseRequestsQuery : IRequest<List<ExpenseRequestDto>>
{
    public bool? Mine { get; init; }
    public int? ForEmployeeId { get; init; }
    public RequestStatus? Status { get; init; }
}