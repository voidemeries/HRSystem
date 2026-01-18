using HRSystem.Application.Requests.ExpenseRequests.Common;
using MediatR;

namespace HRSystem.Application.Requests.ExpenseRequests.Queries.GetExpenseRequestById;

public record GetExpenseRequestByIdQuery(int Id) : IRequest<ExpenseRequestDto>;