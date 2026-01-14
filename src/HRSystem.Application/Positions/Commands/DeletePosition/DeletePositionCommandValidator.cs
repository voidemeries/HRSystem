using FluentValidation;
using HRSystem.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRSystem.Application.Positions.Commands.DeletePosition;

public class DeletePositionCommandValidator : AbstractValidator<DeletePositionCommand>
{
    private readonly IApplicationDbContext _context;

    public DeletePositionCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(v => v.Id)
            .MustAsync(PositionExists).WithMessage("Position does not exist.")
            .MustAsync(NotHaveChildren).WithMessage("Cannot delete position with child positions.")
            .MustAsync(NotHaveEmployees).WithMessage("Cannot delete position with employees.");
    }

    private async Task<bool> PositionExists(int id, CancellationToken cancellationToken)
    {
        return await _context.Positions.AnyAsync(p => p.Id == id, cancellationToken);
    }

    private async Task<bool> NotHaveChildren(int id, CancellationToken cancellationToken)
    {
        return !await _context.Positions.AnyAsync(p => p.ParentPositionId == id, cancellationToken);
    }

    private async Task<bool> NotHaveEmployees(int id, CancellationToken cancellationToken)
    {
        return !await _context.Employees.AnyAsync(e => e.PositionId == id, cancellationToken);
    }
}