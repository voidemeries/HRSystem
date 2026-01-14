using FluentValidation;
using HRSystem.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRSystem.Application.Positions.Commands.UpdatePosition;

public class UpdatePositionCommandValidator : AbstractValidator<UpdatePositionCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdatePositionCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(v => v.Id)
            .NotEmpty().WithMessage("Id is required.")
            .MustAsync(PositionExists).WithMessage("Position does not exist.");

        RuleFor(v => v.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");

        RuleFor(v => v.Code)
            .MaximumLength(50).WithMessage("Code must not exceed 50 characters.")
            .MustAsync(BeUniqueCode).WithMessage("Code already exists.")
                .When(v => !string.IsNullOrWhiteSpace(v.Code));

        RuleFor(v => v.ParentPositionId)
            .MustAsync(ParentExists).WithMessage("Parent position does not exist.")
                .When(v => v.ParentPositionId.HasValue)
            .Must((command, parentId) => parentId != command.Id)
                .WithMessage("Position cannot be its own parent.")
                .When(v => v.ParentPositionId.HasValue);
    }

    private async Task<bool> PositionExists(int id, CancellationToken cancellationToken)
    {
        return await _context.Positions.AnyAsync(p => p.Id == id, cancellationToken);
    }

    private async Task<bool> BeUniqueCode(UpdatePositionCommand command, string? code, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(code))
            return true;

        return !await _context.Positions
            .AnyAsync(p => p.Code == code && p.Id != command.Id, cancellationToken);
    }

    private async Task<bool> ParentExists(int? parentId, CancellationToken cancellationToken)
    {
        if (!parentId.HasValue)
            return true;

        return await _context.Positions
            .AnyAsync(p => p.Id == parentId.Value, cancellationToken);
    }
}