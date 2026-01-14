using FluentValidation;
using HRSystem.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRSystem.Application.Organizations.Commands.DeleteOrganization;

public class DeleteOrganizationCommandValidator : AbstractValidator<DeleteOrganizationCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteOrganizationCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(v => v.Id)
            .MustAsync(OrganizationExists).WithMessage("Organization does not exist.")
            .MustAsync(NotHaveChildren).WithMessage("Cannot delete organization with child organizations.")
            .MustAsync(NotHaveEmployees).WithMessage("Cannot delete organization with employees.");
    }

    private async Task<bool> OrganizationExists(int id, CancellationToken cancellationToken)
    {
        return await _context.Organizations.AnyAsync(o => o.Id == id, cancellationToken);
    }

    private async Task<bool> NotHaveChildren(int id, CancellationToken cancellationToken)
    {
        return !await _context.Organizations.AnyAsync(o => o.ParentOrganizationId == id, cancellationToken);
    }

    private async Task<bool> NotHaveEmployees(int id, CancellationToken cancellationToken)
    {
        return !await _context.Employees.AnyAsync(e => e.OrganizationId == id, cancellationToken);
    }
}