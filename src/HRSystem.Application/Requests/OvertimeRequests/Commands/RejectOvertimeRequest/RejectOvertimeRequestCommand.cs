namespace HRSystem.Application.Requests.OvertimeRequests.Commands.RejectOvertimeRequest;

using HRSystem.Application.Requests.OvertimeRequests.Common;
using MediatR;

public record RejectOvertimeRequestCommand : IRequest<OvertimeRequestDto>
{
    public int Id { get; init; }
    public string RejectionReason { get; init; } = string.Empty;
}