namespace HRSystem.Application.Requests.TravelRequests.Commands.RejectTravelRequest;

using HRSystem.Application.Requests.TravelRequests.Common;
using MediatR;

public record RejectTravelRequestCommand : IRequest<TravelRequestDto>
{
    public int Id { get; init; }
    public string RejectionReason { get; init; } = string.Empty;
}