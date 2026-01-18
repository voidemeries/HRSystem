namespace HRSystem.Application.Requests.LeaveRequests.Commands.RejectLeaveRequest;

using HRSystem.Application.Requests.LeaveRequests.Common;
using MediatR;

public record RejectLeaveRequestCommand : IRequest<LeaveRequestDto>
{
    public int Id { get; init; }
    public string RejectionReason { get; init; } = string.Empty;
}