using FluentValidation;
using HRSystem.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRSystem.Application.Organizations.Commands.UpdateOrganization;

public class UpdateOrganizationCommandValidator : AbstractValidator<UpdateOrganizationCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateOrganizationCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(v => v.Id)
            .NotEmpty().WithMessage("Id is required.")
            .MustAsync(OrganizationExists).WithMessage("Organization does not exist.");

        RuleFor(v => v.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");

        RuleFor(v => v.Code)
            .MaximumLength(50).WithMessage("Code must not exceed 50 characters.")
            .MustAsync(BeUniqueCode).WithMessage("Code already exists.")
                .When(v => !string.IsNullOrWhiteSpace(v.Code));

        RuleFor(v => v.ParentOrganizationId)
            .MustAsync(ParentExists).WithMessage("Parent organization does not exist.")
                .When(v => v.ParentOrganizationId.HasValue)
            .Must((command, parentId) => parentId != command.Id)
                .WithMessage("Organization cannot be its own parent.")
                .When(v => v.ParentOrganizationId.HasValue);
    }

    private async Task<bool> OrganizationExists(int id, CancellationToken cancellationToken)
    {
        return await _context.Organizations.AnyAsync(o => o.Id == id, cancellationToken);
    }

    private async Task<bool> BeUniqueCode(UpdateOrganizationCommand command, string? code, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(code))
            return true;

        return !await _context.Organizations
            .AnyAsync(o => o.Code == code && o.Id != command.Id, cancellationToken);
    }

    private async Task<bool> ParentExists(int? parentId, CancellationToken cancellationToken)
    {
        if (!parentId.HasValue)
            return true;

        return await _context.Organizations
            .AnyAsync(o => o.Id == parentId.Value, cancellationToken);
    }
}