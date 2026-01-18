namespace HRSystem.Application.Requests.LeaveRequests.Commands.ApproveLeaveRequest;

using HRSystem.Application.Requests.LeaveRequests.Common;
using MediatR;

public record ApproveLeaveRequestCommand : IRequest<LeaveRequestDto>
{
    public int Id { get; init; }
}