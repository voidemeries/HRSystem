using HRSystem.Application.Positions.Common;
using MediatR;

namespace HRSystem.Application.Positions.Commands.CreatePosition;

public record CreatePositionCommand : IRequest<PositionDto>
{
    public string Name { get; init; } = string.Empty;
    public string? Code { get; init; }
    public int? ParentPositionId { get; init; }
}