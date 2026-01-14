using HRSystem.Application.Positions.Common;
using MediatR;

namespace HRSystem.Application.Positions.Queries.GetPositionById;

public record GetPositionByIdQuery(int Id) : IRequest<PositionDto>;