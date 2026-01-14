using HRSystem.Application.Positions.Common;
using MediatR;

namespace HRSystem.Application.Positions.Queries.GetPositionsFlat;

public record GetPositionsFlatQuery : IRequest<List<PositionFlatDto>>;