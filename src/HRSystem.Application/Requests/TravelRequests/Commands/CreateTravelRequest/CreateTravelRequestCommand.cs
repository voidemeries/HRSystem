namespace HRSystem.Application.Requests.TravelRequests.Commands.CreateTravelRequest;

using HRSystem.Application.Requests.TravelRequests.Common;
using MediatR;

public record CreateTravelRequestCommand : IRequest<TravelRequestDto>
{
    public int? ForEmployeeId { get; init; }
    public string Destination { get; init; } = string.Empty;
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public string Purpose { get; init; } = string.Empty;
    public decimal EstimatedCost { get; init; }
}