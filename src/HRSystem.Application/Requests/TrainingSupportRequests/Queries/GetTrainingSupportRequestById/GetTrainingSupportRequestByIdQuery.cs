namespace HRSystem.Application.Requests.TrainingSupportRequests.Queries.GetTrainingSupportRequestById;

using HRSystem.Application.Requests.TrainingSupportRequests.Common;
using MediatR;

public record GetTrainingSupportRequestByIdQuery(int Id) : IRequest<TrainingSupportRequestDto>;