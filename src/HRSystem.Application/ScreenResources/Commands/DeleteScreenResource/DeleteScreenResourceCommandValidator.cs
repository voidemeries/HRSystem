using FluentValidation;
using HRSystem.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRSystem.Application.ScreenResources.Commands.DeleteScreenResource;

public class DeleteScreenResourceCommandValidator : AbstractValidator<DeleteScreenResourceCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteScreenResourceCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(v => v.Id)
            .MustAsync(ScreenExists).WithMessage("Screen resource does not exist.")
            .MustAsync(NotHaveChildren).WithMessage("Cannot delete screen resource with child screens.");
    }

    private async Task<bool> ScreenExists(int id, CancellationToken cancellationToken)
    {
        return await _context.ScreenResources.AnyAsync(s => s.Id == id, cancellationToken);
    }

    private async Task<bool> NotHaveChildren(int id, CancellationToken cancellationToken)
    {
        return !await _context.ScreenResources.AnyAsync(s => s.ParentScreenId == id, cancellationToken);
    }
}