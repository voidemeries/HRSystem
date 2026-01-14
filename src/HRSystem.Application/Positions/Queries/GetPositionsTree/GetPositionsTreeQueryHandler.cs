using HRSystem.Application.Common.Interfaces;
using HRSystem.Application.Positions.Common;
using HRSystem.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRSystem.Application.Positions.Queries.GetPositionsTree;

public class GetPositionsTreeQueryHandler : IRequestHandler<GetPositionsTreeQuery, List<PositionTreeDto>>
{
    private readonly IApplicationDbContext _context;

    public GetPositionsTreeQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<PositionTreeDto>> Handle(GetPositionsTreeQuery request, CancellationToken cancellationToken)
    {
        var positions = await _context.Positions
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);

        return BuildTree(positions, null);
    }

    private List<PositionTreeDto> BuildTree(List<Position> allPositions, int? parentId)
    {
        return allPositions
            .Where(p => p.ParentPositionId == parentId)
            .Select(p => new PositionTreeDto
            {
                Id = p.Id,
                Name = p.Name,
                Code = p.Code,
                ParentPositionId = p.ParentPositionId,
                Children = BuildTree(allPositions, p.Id)
            })
            .ToList();
    }
}