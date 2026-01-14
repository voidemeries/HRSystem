using HRSystem.Application.Positions.Common;
using MediatR;

namespace HRSystem.Application.Positions.Queries.GetPositionsTree;

public record GetPositionsTreeQuery : IRequest<List<PositionTreeDto>>;