using HRSystem.Application.Common.Interfaces;
using HRSystem.Application.Permissions.Common;
using HRSystem.Domain.Entities;
using HRSystem.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRSystem.Application.Permissions.Queries.GetPermissionAssignments;

public class GetPermissionAssignmentsQueryHandler : IRequestHandler<GetPermissionAssignmentsQuery, List<PermissionAssignmentDto>>
{
    private readonly IApplicationDbContext _context;

    public GetPermissionAssignmentsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<PermissionAssignmentDto>> Handle(GetPermissionAssignmentsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.PermissionAssignments
            .Include(p => p.ScreenResource)
            .AsQueryable();

        if (request.ScreenResourceId.HasValue)
        {
            query = query.Where(p => p.ScreenResourceId == request.ScreenResourceId.Value);
        }

        if (request.ScopeType.HasValue)
        {
            query = query.Where(p => p.ScopeType == request.ScopeType.Value);
        }

        if (request.ScopeId.HasValue)
        {
            query = query.Where(p => p.ScopeId == request.ScopeId.Value);
        }

        if (request.PermissionType.HasValue)
        {
            query = query.Where(p => p.PermissionType == request.PermissionType.Value);
        }

        var permissions = await query
            .OrderBy(p => p.ScreenResource.Name)
            .ThenBy(p => p.ScopeType)
            .ToListAsync(cancellationToken);

        var result = new List<PermissionAssignmentDto>();

        foreach (var permission in permissions)
        {
            var scopeName = await GetScopeNameAsync(permission.ScopeType, permission.ScopeId, cancellationToken);

            result.Add(new PermissionAssignmentDto
            {
                Id = permission.Id,
                ScreenResourceId = permission.ScreenResourceId,
                ScreenName = permission.ScreenResource.Name,
                ScopeType = permission.ScopeType.ToString(),
                ScopeId = permission.ScopeId,
                ScopeName = scopeName,
                PermissionType = permission.PermissionType.ToString(),
                CreatedAt = permission.CreatedAt,
                UpdatedAt = permission.UpdatedAt
            });
        }

        return result;
    }

    private async Task<string> GetScopeNameAsync(ScopeType scopeType, int scopeId, CancellationToken cancellationToken)
    {
        return scopeType switch
        {
            ScopeType.Organization => (await _context.Organizations.FirstOrDefaultAsync(o => o.Id == scopeId, cancellationToken))?.Name ?? "Unknown",
            ScopeType.Position => (await _context.Positions.FirstOrDefaultAsync(p => p.Id == scopeId, cancellationToken))?.Name ?? "Unknown",
            ScopeType.Employee => (await _context.Employees.FirstOrDefaultAsync(e => e.Id == scopeId, cancellationToken)) is Employee emp
                ? $"{emp.FirstName} {emp.LastName}"
                : "Unknown",
            _ => "Unknown"
        };
    }
}