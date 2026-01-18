namespace HRSystem.Application.Requests.TravelRequests.Queries.GetTravelRequests;

using HRSystem.Application.Requests.TravelRequests.Common;
using HRSystem.Domain.Enums;
using MediatR;

public record GetTravelRequestsQuery : IRequest<List<TravelRequestDto>>
{
    public bool? Mine { get; init; }
    public int? ForEmployeeId { get; init; }
    public RequestStatus? Status { get; init; }
}