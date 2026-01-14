using MediatR;

namespace HRSystem.Application.Positions.Commands.DeletePosition;

public record DeletePositionCommand(int Id) : IRequest<Unit>;