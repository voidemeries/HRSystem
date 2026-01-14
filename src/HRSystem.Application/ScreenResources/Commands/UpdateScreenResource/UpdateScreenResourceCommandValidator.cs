using FluentValidation;
using HRSystem.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRSystem.Application.ScreenResources.Commands.UpdateScreenResource;

public class UpdateScreenResourceCommandValidator : AbstractValidator<UpdateScreenResourceCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateScreenResourceCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(v => v.Id)
            .MustAsync(ScreenExists).WithMessage("Screen resource does not exist.");

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
            .When(v => v.ParentScreenId.HasValue)
            .Must((command, parentId) => parentId != command.Id)
            .WithMessage("Screen cannot be its own parent.")
            .When(v => v.ParentScreenId.HasValue)
            .MustAsync(NotCreateCircularReference).WithMessage("Circular reference detected in screen hierarchy.")
            .When(v => v.ParentScreenId.HasValue);
    }

    private async Task<bool> ScreenExists(int id, CancellationToken cancellationToken)
    {
        return await _context.ScreenResources.AnyAsync(s => s.Id == id, cancellationToken);
    }

    private async Task<bool> BeUniqueRoutePath(UpdateScreenResourceCommand command, string routePath, CancellationToken cancellationToken)
    {
        return !await _context.ScreenResources
            .AnyAsync(s => s.RoutePath == routePath && s.Id != command.Id, cancellationToken);
    }

    private async Task<bool> ParentExists(int? parentId, CancellationToken cancellationToken)
    {
        if (!parentId.HasValue)
            return true;

        return await _context.ScreenResources
            .AnyAsync(s => s.Id == parentId.Value, cancellationToken);
    }

    private async Task<bool> NotCreateCircularReference(UpdateScreenResourceCommand command, int? parentId, CancellationToken cancellationToken)
    {
        if (!parentId.HasValue)
            return true;

        // Check if the new parent is a descendant of this screen
        var allScreens = await _context.ScreenResources.ToListAsync(cancellationToken);
        return !IsDescendant(command.Id, parentId.Value, allScreens);
    }

    private bool IsDescendant(int screenId, int potentialDescendantId, List<Domain.Entities.ScreenResource> allScreens)
    {
        var current = allScreens.FirstOrDefault(s => s.Id == potentialDescendantId);

        while (current != null)
        {
            if (current.ParentScreenId == screenId)
                return true;

            current = allScreens.FirstOrDefault(s => s.Id == current.ParentScreenId);
        }

        return false;
    }
}