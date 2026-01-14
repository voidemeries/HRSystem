using FluentValidation;
using HRSystem.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRSystem.Application.ScreenResources.Commands.CreateScreenResource;

public class CreateScreenResourceCommandValidator : AbstractValidator<CreateScreenResourceCommand>
{
    private readonly IApplicationDbContext _context;

    public CreateScreenResourceCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(v => v.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");

        RuleFor(v => v.RoutePath)
            .NotEmpty().WithMessage("Route path is required.")
            .MaximumLength(500).WithMessage("Route path must not exceed 500 characters.")
            .MustAsync(BeUniqueRoutePath).WithMessage("Route path already exists.");

        RuleFor(v => v.Icon)
            .MaximumLength(100).WithMessage("Icon must not exceed 100 characters.")
            .When(v => !string.IsNullOrWhiteSpace(v.Icon));

        RuleFor(v => v.ParentScreenId)
            .MustAsync(ParentExists).WithMessage("Parent screen does not exist.")
            .When(v => v.ParentScreenId.HasValue);
    }

    private async Task<bool> BeUniqueRoutePath(string routePath, CancellationToken cancellationToken)
    {
        return !await _context.ScreenResources
            .AnyAsync(s => s.RoutePath == routePath, cancellationToken);
    }

    private async Task<bool> ParentExists(int? parentId, CancellationToken cancellationToken)
    {
        if (!parentId.HasValue)
            return true;

        return await _context.ScreenResources
            .AnyAsync(s => s.Id == parentId.Value, cancellationToken);
    }
}