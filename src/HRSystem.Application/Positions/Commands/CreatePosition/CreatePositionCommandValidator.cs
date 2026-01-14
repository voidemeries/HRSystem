using FluentValidation;
using HRSystem.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRSystem.Application.Positions.Commands.CreatePosition;

public class CreatePositionCommandValidator : AbstractValidator<CreatePositionCommand>
{
    private readonly IApplicationDbContext _context;

    public CreatePositionCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(v => v.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");

        RuleFor(v => v.Code)
            .MaximumLength(50).WithMessage("Code must not exceed 50 characters.")
            .MustAsync(BeUniqueCode).WithMessage("Code already exists.")
                .When(v => !string.IsNullOrWhiteSpace(v.Code));

        RuleFor(v => v.ParentPositionId)
            .MustAsync(ParentExists).WithMessage("Parent position does not exist.")
                .When(v => v.ParentPositionId.HasValue);
    }

    private async Task<bool> BeUniqueCode(string? code, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(code))
            return true;

        return !await _context.Positions
            .AnyAsync(p => p.Code == code, cancellationToken);
    }

    private async Task<bool> ParentExists(int? parentId, CancellationToken cancellationToken)
    {
        if (!parentId.HasValue)
            return true;

        return await _context.Positions
            .AnyAsync(p => p.Id == parentId.Value, cancellationToken);
    }
}