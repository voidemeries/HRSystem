using FluentValidation;
using HRSystem.Application.Common.Interfaces;
using HRSystem.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HRSystem.Application.Permissions.Commands.CreatePermissionAssignment;

public class CreatePermissionAssignmentCommandValidator : AbstractValidator<CreatePermissionAssignmentCommand>
{
    private readonly IApplicationDbContext _context;

    public CreatePermissionAssignmentCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(v => v.ScreenResourceId)
            .MustAsync(ScreenResourceExists).WithMessage("Screen resource does not exist.");

        RuleFor(v => v.ScopeId)
            .MustAsync(ScopeExists).WithMessage("Scope entity does not exist.");

        RuleFor(v => v)
            .MustAsync(BeUniqueAssignment).WithMessage("Permission assignment already exists for this combination.");
    }

    private async Task<bool> ScreenResourceExists(int screenResourceId, CancellationToken cancellationToken)
    {
        return await _context.ScreenResources.AnyAsync(s => s.Id == screenResourceId, cancellationToken);
    }

    private async Task<bool> ScopeExists(CreatePermissionAssignmentCommand command, int scopeId, CancellationToken cancellationToken)
    {
        return command.ScopeType switch
        {
            ScopeType.Organization => await _context.Organizations.AnyAsync(o => o.Id == scopeId, cancellationToken),
            ScopeType.Position => await _context.Positions.AnyAsync(p => p.Id == scopeId, cancellationToken),
            ScopeType.Employee => await _context.Employees.AnyAsync(e => e.Id == scopeId, cancellationToken),
            _ => false
        };
    }

    private async Task<bool> BeUniqueAssignment(CreatePermissionAssignmentCommand command, CancellationToken cancellationToken)
    {
        return !await _context.PermissionAssignments.AnyAsync(p =>
            p.ScreenResourceId == command.ScreenResourceId &&
            p.ScopeType == command.ScopeType &&
            p.ScopeId == command.ScopeId &&
            p.PermissionType == command.PermissionType, cancellationToken);
    }
}