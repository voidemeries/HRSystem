namespace HRSystem.Application.Requests.OvertimeRequests.Commands.ApproveOvertimeRequest;

using HRSystem.Application.Requests.OvertimeRequests.Common;
using MediatR;

public record ApproveOvertimeRequestCommand : IRequest<OvertimeRequestDto>
{
    public int Id { get; init; }
}