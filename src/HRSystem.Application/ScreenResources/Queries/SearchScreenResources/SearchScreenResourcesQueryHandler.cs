using HRSystem.Application.Common.Interfaces;
using HRSystem.Application.ScreenResources.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRSystem.Application.ScreenResources.Queries.SearchScreenResources;

public class SearchScreenResourcesQueryHandler : IRequestHandler<SearchScreenResourcesQuery, List<ScreenResourceSearchResultDto>>
{
    private readonly IApplicationDbContext _context;

    public SearchScreenResourcesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ScreenResourceSearchResultDto>> Handle(SearchScreenResourcesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.ScreenResources
            .Include(s => s.ParentScreen)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Query))
        {
            var searchTerm = request.Query.ToLower();
            query = query.Where(s =>
                s.Name.ToLower().Contains(searchTerm) ||
                s.RoutePath.ToLower().Contains(searchTerm));
        }

        var results = await query
            .OrderBy(s => s.Name)
            .Select(s => new ScreenResourceSearchResultDto
            {
                Id = s.Id,
                Name = s.Name,
                RoutePath = s.RoutePath,
                ParentScreenId = s.ParentScreenId,
                ParentScreenName = s.ParentScreen != null ? s.ParentScreen.Name : null,
                IsActive = s.IsActive,
                Icon = s.Icon
            })
            .ToListAsync(cancellationToken);

        return results;
    }
}