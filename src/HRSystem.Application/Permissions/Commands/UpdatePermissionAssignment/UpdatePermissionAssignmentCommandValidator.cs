using FluentValidation;
using HRSystem.Application.Common.Interfaces;
using HRSystem.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HRSystem.Application.Permissions.Commands.UpdatePermissionAssignment;

public class UpdatePermissionAssignmentCommandValidator : AbstractValidator<UpdatePermissionAssignmentCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdatePermissionAssignmentCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(v => v.Id)
            .MustAsync(PermissionExists).WithMessage("Permission assignment does not exist.");

        RuleFor(v => v.ScreenResourceId)
            .MustAsync(ScreenResourceExists).WithMessage("Screen resource does not exist.");

        RuleFor(v => v.ScopeId)
            .MustAsync(ScopeExists).WithMessage("Scope entity does not exist.");

        RuleFor(v => v)
            .MustAsync(BeUniqueAssignment).WithMessage("Permission assignment already exists for this combination.");
    }

    private async Task<bool> PermissionExists(int id, CancellationToken cancellationToken)
    {
        return await _context.PermissionAssignments.AnyAsync(p => p.Id == id, cancellationToken);
    }

    private async Task<bool> ScreenResourceExists(int screenResourceId, CancellationToken cancellationToken)
    {
        return await _context.ScreenResources.AnyAsync(s => s.Id == screenResourceId, cancellationToken);
    }

    private async Task<bool> ScopeExists(UpdatePermissionAssignmentCommand command, int scopeId, CancellationToken cancellationToken)
    {
        return command.ScopeType switch
        {
            ScopeType.Organization => await _context.Organizations.AnyAsync(o => o.Id == scopeId, cancellationToken),
            ScopeType.Position => await _context.Positions.AnyAsync(p => p.Id == scopeId, cancellationToken),
            ScopeType.Employee => await _context.Employees.AnyAsync(e => e.Id == scopeId, cancellationToken),
            _ => false
        };
    }

    private async Task<bool> BeUniqueAssignment(UpdatePermissionAssignmentCommand command, CancellationToken cancellationToken)
    {
        return !await _context.PermissionAssignments.AnyAsync(p =>
            p.ScreenResourceId == command.ScreenResourceId &&
            p.ScopeType == command.ScopeType &&
            p.ScopeId == command.ScopeId &&
            p.PermissionType == command.PermissionType &&
            p.Id != command.Id, cancellationToken);
    }
}