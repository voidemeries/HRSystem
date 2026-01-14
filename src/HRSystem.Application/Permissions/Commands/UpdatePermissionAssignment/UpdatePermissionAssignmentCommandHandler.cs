using HRSystem.Application.Common.Interfaces;
using HRSystem.Application.Permissions.Common;
using HRSystem.Domain.Entities;
using HRSystem.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRSystem.Application.Permissions.Commands.UpdatePermissionAssignment;

public class UpdatePermissionAssignmentCommandHandler : IRequestHandler<UpdatePermissionAssignmentCommand, PermissionAssignmentDto>
{
    private readonly IApplicationDbContext _context;

    public UpdatePermissionAssignmentCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PermissionAssignmentDto> Handle(UpdatePermissionAssignmentCommand request, CancellationToken cancellationToken)
    {
        var permission = await _context.PermissionAssignments
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Permission assignment with ID {request.Id} not found.");

        permission.ScreenResourceId = request.ScreenResourceId;
        permission.ScopeType = request.ScopeType;
        permission.ScopeId = request.ScopeId;
        permission.PermissionType = request.PermissionType;

        await _context.SaveChangesAsync(cancellationToken);

        var screenResource = await _context.ScreenResources
            .FirstAsync(s => s.Id == permission.ScreenResourceId, cancellationToken);

        var scopeName = await GetScopeNameAsync(permission.ScopeType, permission.ScopeId, cancellationToken);

        return new PermissionAssignmentDto
        {
            Id = permission.Id,
            ScreenResourceId = permission.ScreenResourceId,
            ScreenName = screenResource.Name,
            ScopeType = permission.ScopeType.ToString(),
            ScopeId = permission.ScopeId,
            ScopeName = scopeName,
            PermissionType = permission.PermissionType.ToString(),
            CreatedAt = permission.CreatedAt,
            UpdatedAt = permission.UpdatedAt
        };
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