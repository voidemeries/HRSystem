namespace HRSystem.Application.Requests.LeaveRequests.Queries.GetLeaveRequestById;

using HRSystem.Application.Requests.LeaveRequests.Common;
using MediatR;

public record GetLeaveRequestByIdQuery(int Id) : IRequest<LeaveRequestDto>;