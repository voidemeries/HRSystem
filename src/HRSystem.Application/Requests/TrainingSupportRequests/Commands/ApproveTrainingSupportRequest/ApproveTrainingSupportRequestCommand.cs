namespace HRSystem.Application.Requests.TrainingSupportRequests.Commands.ApproveTrainingSupportRequest;

using HRSystem.Application.Requests.TrainingSupportRequests.Common;
using MediatR;

public record ApproveTrainingSupportRequestCommand : IRequest<TrainingSupportRequestDto>
{
    public int Id { get; init; }
}