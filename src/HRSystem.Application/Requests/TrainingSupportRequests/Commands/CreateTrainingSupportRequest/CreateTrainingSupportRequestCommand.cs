using HRSystem.Application.Requests.TrainingSupportRequests.Common;
using MediatR;

namespace HRSystem.Application.Requests.TrainingSupportRequests.Commands.CreateTrainingSupportRequest;

public record CreateTrainingSupportRequestCommand : IRequest<TrainingSupportRequestDto>
{
    public int? ForEmployeeId { get; init; }
    public string TrainingName { get; init; } = string.Empty;
    public string Provider { get; init; } = string.Empty;
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public decimal Cost { get; init; }
    public string Justification { get; init; } = string.Empty;
}