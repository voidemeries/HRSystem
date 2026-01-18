using HRSystem.Application.Requests.OvertimeRequests.Common;
using MediatR;

namespace HRSystem.Application.Requests.OvertimeRequests.Commands.CreateOvertimeRequest;

public record CreateOvertimeRequestCommand : IRequest<OvertimeRequestDto>
{
    public int? ForEmployeeId { get; init; }
    public DateTime OvertimeDate { get; init; }
    public decimal Hours { get; init; }
    public string Reason { get; init; } = string.Empty;
}