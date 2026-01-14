using HRSystem.Application.Common.Interfaces;
using HRSystem.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HRSystem.Infrastructure.Services;

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

        var permissions = new List<PermissionType>();

        // Get all permission assignments for this screen
        var assignments = await _context.PermissionAssignments
            .Where(p => p.ScreenResourceId == screenResourceId)
            .ToListAsync(cancellationToken);

        // Employee-level permissions (highest precedence)
        var employeePermissions = assignments
            .Where(p => p.ScopeType == ScopeType.Employee && p.ScopeId == employeeId)
            .Select(p => p.PermissionType)
            .ToList();

        if (employeePermissions.Any())
        {
            permissions.AddRange(employeePermissions);
        }
        else
        {
            // Position-level permissions (second precedence)
            var positionPermissions = assignments
                .Where(p => p.ScopeType == ScopeType.Position && p.ScopeId == employee.PositionId)
                .Select(p => p.PermissionType)
                .ToList();

            if (positionPermissions.Any())
            {
                permissions.AddRange(positionPermissions);
            }
            else
            {
                // Organization-level permissions (lowest precedence)
                var organizationPermissions = assignments
                    .Where(p => p.ScopeType == ScopeType.Organization && p.ScopeId == employee.OrganizationId)
                    .Select(p => p.PermissionType)
                    .ToList();

                permissions.AddRange(organizationPermissions);
            }
        }

        return permissions.Distinct().ToList();
    }

    public async Task<Dictionary<int, List<PermissionType>>> GetUserPermissionsForAllScreensAsync(
        int employeeId,
        CancellationToken cancellationToken = default)
    {
        var employee = await _context.Employees
            .FirstOrDefaultAsync(e => e.Id == employeeId, cancellationToken);

        if (employee == null)
            return new Dictionary<int, List<PermissionType>>();

        var allAssignments = await _context.PermissionAssignments
            .ToListAsync(cancellationToken);

        var screenIds = allAssignments.Select(p => p.ScreenResourceId).Distinct();
        var result = new Dictionary<int, List<PermissionType>>();

        foreach (var screenId in screenIds)
        {
            var screenAssignments = allAssignments.Where(p => p.ScreenResourceId == screenId).ToList();
            var permissions = new List<PermissionType>();

            // Employee-level (highest precedence)
            var employeePerms = screenAssignments
                .Where(p => p.ScopeType == ScopeType.Employee && p.ScopeId == employeeId)
                .Select(p => p.PermissionType)
                .ToList();

            if (employeePerms.Any())
            {
                permissions.AddRange(employeePerms);
            }
            else
            {
                // Position-level
                var positionPerms = screenAssignments
                    .Where(p => p.ScopeType == ScopeType.Position && p.ScopeId == employee.PositionId)
                    .Select(p => p.PermissionType)
                    .ToList();

                if (positionPerms.Any())
                {
                    permissions.AddRange(positionPerms);
                }
                else
                {
                    // Organization-level
                    var orgPerms = screenAssignments
                        .Where(p => p.ScopeType == ScopeType.Organization && p.ScopeId == employee.OrganizationId)
                        .Select(p => p.PermissionType)
                        .ToList();

                    permissions.AddRange(orgPerms);
                }
            }

            if (permissions.Any())
            {
                result[screenId] = permissions.Distinct().ToList();
            }
        }

        return result;
    }

    public async Task<List<int>> GetAuthorizedScreenIdsAsync(
        int employeeId,
        CancellationToken cancellationToken = default)
    {
        var permissions = await GetUserPermissionsForAllScreensAsync(employeeId, cancellationToken);
        return permissions
            .Where(kvp => kvp.Value.Contains(PermissionType.View) ||
                         kvp.Value.Contains(PermissionType.Manage) ||
                         kvp.Value.Contains(PermissionType.Approve) ||
                         kvp.Value.Contains(PermissionType.AdminOverride))
            .Select(kvp => kvp.Key)
            .ToList();
    }
}