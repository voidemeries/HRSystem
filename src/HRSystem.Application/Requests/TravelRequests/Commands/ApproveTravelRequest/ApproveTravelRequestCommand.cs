namespace HRSystem.Application.Requests.TravelRequests.Commands.ApproveTravelRequest;

using HRSystem.Application.Requests.TravelRequests.Common;
using MediatR;

public record ApproveTravelRequestCommand : IRequest<TravelRequestDto>
{
    public int Id { get; init; }
}