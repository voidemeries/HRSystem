using HRSystem.Application.Positions.Common;
using MediatR;

namespace HRSystem.Application.Positions.Commands.UpdatePosition;

public record UpdatePositionCommand : IRequest<PositionDto>
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Code { get; init; }
    public int? ParentPositionId { get; init; }
}