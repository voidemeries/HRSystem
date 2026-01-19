namespace HRSystem.Application.Requests.TrainingSupportRequests.Commands.RejectTrainingSupportRequest;

using HRSystem.Application.Requests.TrainingSupportRequests.Common;
using MediatR;

public record RejectTrainingSupportRequestCommand : IRequest<TrainingSupportRequestDto>
{
    public int Id { get; init; }
    public string RejectionReason { get; init; } = string.Empty;
}