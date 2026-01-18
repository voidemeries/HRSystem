namespace HRSystem.Application.Requests.TravelRequests.Queries.GetTravelRequestById;

using HRSystem.Application.Requests.TravelRequests.Common;
using MediatR;

public record GetTravelRequestByIdQuery(int Id) : IRequest<TravelRequestDto>;