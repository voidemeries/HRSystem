using HRSystem.Application.Common.Interfaces;
using HRSystem.Application.Positions.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRSystem.Application.Positions.Queries.GetPositionsFlat;

public class GetPositionsFlatQueryHandler : IRequestHandler<GetPositionsFlatQuery, List<PositionFlatDto>>
{
    private readonly IApplicationDbContext _context;

    public GetPositionsFlatQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<PositionFlatDto>> Handle(GetPositionsFlatQuery request, CancellationToken cancellationToken)
    {
        return await _context.Positions
            .OrderBy(p => p.Name)
            .Select(p => new PositionFlatDto
            {
                Id = p.Id,
                Name = p.Name,
                Code = p.Code,
                ParentPositionId = p.ParentPositionId
            })
            .ToListAsync(cancellationToken);
    }
}