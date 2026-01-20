using HRSystem.Application.Common.Interfaces;
using HRSystem.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HRSystem.Infrastructure.Services;

/// <summary>
/// Service for resolving user permissions based on scope assignments
/// Permission Resolution Logic:
/// 1. Admin users have all permissions to all screens automatically
/// 2. For non-admin users, permissions are accumulated from all applicable scopes:
///    - Permissions assigned to the employee directly (ScopeType.Employee)
///    - Permissions assigned to the employee's position (ScopeType.Position)
///    - Permissions assigned to the employee's organization (ScopeType.Organization)
/// 3. All permissions from all matching scopes are combined (union)
/// 4. Example: If an employee has View through Organization and Manage through Position,
///    they get both View AND Manage permissions
/// </summary>
public class PermissionService : IPermissionService
{
    private readonly IApplicationDbContext _context;

    public PermissionService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<PermissionType>> GetUserPermissionsForScreenAsync(
        int employeeId,
        int screenResourceId,
        CancellationToken cancellationToken = default)
    {
        var employee = await _context.Employees
            .Include(e => e.Organization)
            .Include(e => e.Position)
            .FirstOrDefaultAsync(e => e.Id == employeeId, cancellationToken);

        if (employee == null)
            return new List<PermissionType>();

        // Admin users have all permissions automatically
        if (employee.IsAdmin)
        {
            return new List<PermissionType>
            {
                PermissionType.View,
                PermissionType.Manage,
                PermissionType.Approve
            };
        }

        var permissions = new HashSet<PermissionType>();

        // Get all permission assignments for this screen
        var assignments = await _context.PermissionAssignments
            .Where(p => p.ScreenResourceId == screenResourceId)
            .ToListAsync(cancellationToken);

        // Collect permissions from Employee scope
        var employeePermissions = assignments
            .Where(p => p.ScopeType == ScopeType.Employee && p.ScopeId == employeeId)
            .Select(p => p.PermissionType);

        foreach (var perm in employeePermissions)
        {
            permissions.Add(perm);
        }

        // Collect permissions from Position scope
        var positionPermissions = assignments
            .Where(p => p.ScopeType == ScopeType.Position && p.ScopeId == employee.PositionId)
            .Select(p => p.PermissionType);

        foreach (var perm in positionPermissions)
        {
            permissions.Add(perm);
        }

        // Collect permissions from Organization scope
        var organizationPermissions = assignments
            .Where(p => p.ScopeType == ScopeType.Organization && p.ScopeId == employee.OrganizationId)
            .Select(p => p.PermissionType);

        foreach (var perm in organizationPermissions)
        {
            permissions.Add(perm);
        }

        return permissions.ToList();
    }

    public async Task<Dictionary<int, List<PermissionType>>> GetUserPermissionsForAllScreensAsync(
        int employeeId,
        CancellationToken cancellationToken = default)
    {
        var employee = await _context.Employees
            .Include(e => e.Organization)
            .Include(e => e.Position)
            .FirstOrDefaultAsync(e => e.Id == employeeId, cancellationToken);

        if (employee == null)
            return new Dictionary<int, List<PermissionType>>();

        var result = new Dictionary<int, List<PermissionType>>();

        // Admin users have all permissions to all screens
        if (employee.IsAdmin)
        {
            var allScreenIds = await _context.ScreenResources
                .Select(s => s.Id)
                .ToListAsync(cancellationToken);

            var allPermissions = new List<PermissionType>
            {
                PermissionType.View,
                PermissionType.Manage,
                PermissionType.Approve
            };

            foreach (var screenId in allScreenIds)
            {
                result[screenId] = allPermissions;
            }

            return result;
        }

        // Get all permission assignments
        var allAssignments = await _context.PermissionAssignments
            .ToListAsync(cancellationToken);

        // Get assignments that apply to this employee
        var applicableAssignments = allAssignments.Where(p =>
            // Employee-specific assignments
            (p.ScopeType == ScopeType.Employee && p.ScopeId == employeeId) ||
            // Position-based assignments
            (p.ScopeType == ScopeType.Position && p.ScopeId == employee.PositionId) ||
            // Organization-based assignments
            (p.ScopeType == ScopeType.Organization && p.ScopeId == employee.OrganizationId)
        ).ToList();

        // Group by screen and collect all permissions
        var screenGroups = applicableAssignments.GroupBy(p => p.ScreenResourceId);

        foreach (var screenGroup in screenGroups)
        {
            var screenId = screenGroup.Key;
            var permissionsForScreen = screenGroup
                .Select(p => p.PermissionType)
                .Distinct()
                .ToList();

            result[screenId] = permissionsForScreen;
        }

        return result;
    }

    public async Task<List<int>> GetAuthorizedScreenIdsAsync(
        int employeeId,
        CancellationToken cancellationToken = default)
    {
        var permissions = await GetUserPermissionsForAllScreensAsync(employeeId, cancellationToken);

        // Return screen IDs where user has at least View permission
        return permissions
            .Where(kvp => kvp.Value.Contains(PermissionType.View))
            .Select(kvp => kvp.Key)
            .ToList();
    }
}