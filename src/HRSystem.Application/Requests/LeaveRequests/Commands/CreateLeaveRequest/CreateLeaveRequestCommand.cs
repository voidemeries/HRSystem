using HRSystem.Application.Requests.LeaveRequests.Common;
using MediatR;

namespace HRSystem.Application.Requests.LeaveRequests.Commands.CreateLeaveRequest;

public record CreateLeaveRequestCommand : IRequest<LeaveRequestDto>
{
    public int? ForEmployeeId { get; init; }
    public string LeaveType { get; init; } = string.Empty;
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public string Reason { get; init; } = string.Empty;
}