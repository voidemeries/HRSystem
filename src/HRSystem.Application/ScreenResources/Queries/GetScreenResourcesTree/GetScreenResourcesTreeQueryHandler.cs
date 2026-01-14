using HRSystem.Application.Common.Interfaces;
using HRSystem.Application.ScreenResources.Common;
using HRSystem.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRSystem.Application.ScreenResources.Queries.GetScreenResourcesTree;

public class GetScreenResourcesTreeQueryHandler : IRequestHandler<GetScreenResourcesTreeQuery, List<ScreenResourceTreeDto>>
{
    private readonly IApplicationDbContext _context;

    public GetScreenResourcesTreeQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ScreenResourceTreeDto>> Handle(GetScreenResourcesTreeQuery request, CancellationToken cancellationToken)
    {
        var screens = await _context.ScreenResources
            .OrderBy(s => s.SortOrder)
            .ThenBy(s => s.Name)
            .ToListAsync(cancellationToken);

        return BuildTree(screens, null);
    }

    private List<ScreenResourceTreeDto> BuildTree(List<ScreenResource> allScreens, int? parentId)
    {
        return allScreens
            .Where(s => s.ParentScreenId == parentId)
            .Select(s => new ScreenResourceTreeDto
            {
                Id = s.Id,
                Name = s.Name,
                RoutePath = s.RoutePath,
                ParentScreenId = s.ParentScreenId,
                IsActive = s.IsActive,
                SortOrder = s.SortOrder,
                Icon = s.Icon,
                Children = BuildTree(allScreens, s.Id)
            })
            .ToList();
    }
}