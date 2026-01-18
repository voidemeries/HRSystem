namespace HRSystem.Application.Requests.TrainingSupportRequests.Queries.GetTrainingSupportRequests;

using HRSystem.Application.Requests.TrainingSupportRequests.Common;
using HRSystem.Domain.Enums;
using MediatR;

public record GetTrainingSupportRequestsQuery : IRequest<List<TrainingSupportRequestDto>>
{
    public bool? Mine { get; init; }
    public int? ForEmployeeId { get; init; }
    public RequestStatus? Status { get; init; }
}