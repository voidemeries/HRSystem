using HRSystem.Application.Common.Interfaces;
using HRSystem.Application.Permissions.Common;
using HRSystem.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRSystem.Application.Permissions.Queries.SearchUserScreens;

public class SearchUserScreensQueryHandler : IRequestHandler<SearchUserScreensQuery, List<UserScreenSearchResultDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IPermissionService _permissionService;

    public SearchUserScreensQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        IPermissionService permissionService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _permissionService = permissionService;
    }

    public async Task<List<UserScreenSearchResultDto>> Handle(SearchUserScreensQuery request, CancellationToken cancellationToken)
    {
        var employeeId = _currentUserService.EmployeeId
            ?? throw new UnauthorizedAccessException("User not authenticated.");

        // Get all user permissions
        var userPermissions = await _permissionService.GetUserPermissionsForAllScreensAsync(employeeId, cancellationToken);

        var query = _context.ScreenResources
            .Include(s => s.ParentScreen)
            .AsQueryable();

        // Apply search filter
        if (!string.IsNullOrWhiteSpace(request.Query))
        {
            var searchTerm = request.Query.ToLower();
            query = query.Where(s =>
                s.Name.ToLower().Contains(searchTerm) ||
                s.RoutePath.ToLower().Contains(searchTerm));
        }

        var allScreens = await query.ToListAsync(cancellationToken);

        // Filter by permissions and IsActive
        var authorizedScreens = allScreens.Where(s =>
        {
            if (!userPermissions.ContainsKey(s.Id))
                return false;

            var permissions = userPermissions[s.Id];

            // AdminOverride sees all
            if (permissions.Contains(PermissionType.AdminOverride))
                return true;

            // Regular users only see active screens
            return s.IsActive;
        }).ToList();

        return authorizedScreens
            .OrderBy(s => s.Name)
            .Select(s => new UserScreenSearchResultDto
            {
                Id = s.Id,
                Name = s.Name,
                RoutePath = s.RoutePath,
                ParentScreenId = s.ParentScreenId,
                ParentScreenName = s.ParentScreen?.Name,
                Icon = s.Icon,
                Permissions = userPermissions[s.Id].Select(p => p.ToString()).ToList()
            })
            .ToList();
    }
}