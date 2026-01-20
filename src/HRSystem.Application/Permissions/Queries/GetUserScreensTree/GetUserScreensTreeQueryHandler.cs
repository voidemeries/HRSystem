using HRSystem.Application.Common.Interfaces;
using HRSystem.Application.Permissions.Common;
using HRSystem.Domain.Entities;
using HRSystem.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRSystem.Application.Permissions.Queries.GetUserScreensTree;

public class GetUserScreensTreeQueryHandler : IRequestHandler<GetUserScreensTreeQuery, List<UserScreenTreeDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IPermissionService _permissionService;

    public GetUserScreensTreeQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        IPermissionService permissionService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _permissionService = permissionService;
    }

    public async Task<List<UserScreenTreeDto>> Handle(GetUserScreensTreeQuery request, CancellationToken cancellationToken)
    {
        var employeeId = _currentUserService.EmployeeId
            ?? throw new UnauthorizedAccessException("User not authenticated.");

        var isAdmin = _currentUserService.IsAdmin;

        // Get all user permissions
        var userPermissions = await _permissionService.GetUserPermissionsForAllScreensAsync(employeeId, cancellationToken);

        // Get all screens
        var allScreens = await _context.ScreenResources
            .OrderBy(s => s.SortOrder)
            .ThenBy(s => s.Name)
            .ToListAsync(cancellationToken);

        // Filter screens based on permissions
        var authorizedScreens = allScreens.Where(s =>
        {
            // Check if user has any permissions for this screen
            if (!userPermissions.ContainsKey(s.Id))
                return false;

            var permissions = userPermissions[s.Id];

            // Admin users see all screens
            if (isAdmin)
                return true;

            // Regular users only see active screens where they have permissions
            return s.IsActive && permissions.Any();
        }).ToList();

        // Build tree
        return BuildTree(authorizedScreens, userPermissions, null);
    }

    private List<UserScreenTreeDto> BuildTree(
        List<ScreenResource> screens,
        Dictionary<int, List<PermissionType>> userPermissions,
        int? parentId)
    {
        return screens
            .Where(s => s.ParentScreenId == parentId)
            .Select(s => new UserScreenTreeDto
            {
                Id = s.Id,
                Name = s.Name,
                RoutePath = s.RoutePath,
                ParentScreenId = s.ParentScreenId,
                Icon = s.Icon,
                SortOrder = s.SortOrder,
                Permissions = userPermissions.ContainsKey(s.Id)
                    ? userPermissions[s.Id].Select(p => p.ToString()).ToList()
                    : new List<string>(),
                Children = BuildTree(screens, userPermissions, s.Id)
            })
            .ToList();
    }
}