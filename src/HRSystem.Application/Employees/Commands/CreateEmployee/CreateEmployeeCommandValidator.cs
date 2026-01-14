using FluentValidation;
using HRSystem.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRSystem.Application.Employees.Commands.CreateEmployee;

public class CreateEmployeeCommandValidator : AbstractValidator<CreateEmployeeCommand>
{
    private readonly IApplicationDbContext _context;

    public CreateEmployeeCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(v => v.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(100).WithMessage("First name must not exceed 100 characters.");

        RuleFor(v => v.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(100).WithMessage("Last name must not exceed 100 characters.");

        RuleFor(v => v.Email)
            .EmailAddress().WithMessage("Invalid email format.")
            .MaximumLength(200).WithMessage("Email must not exceed 200 characters.")
            .When(v => !string.IsNullOrWhiteSpace(v.Email));

        RuleFor(v => v.Phone)
            .MaximumLength(50).WithMessage("Phone must not exceed 50 characters.")
            .When(v => !string.IsNullOrWhiteSpace(v.Phone));

        RuleFor(v => v.OrganizationId)
            .MustAsync(OrganizationExists).WithMessage("Organization does not exist.");

        RuleFor(v => v.PositionId)
            .MustAsync(PositionExists).WithMessage("Position does not exist.");
    }

    private async Task<bool> OrganizationExists(int organizationId, CancellationToken cancellationToken)
    {
        return await _context.Organizations.AnyAsync(o => o.Id == organizationId, cancellationToken);
    }

    private async Task<bool> PositionExists(int positionId, CancellationToken cancellationToken)
    {
        return await _context.Positions.AnyAsync(p => p.Id == positionId, cancellationToken);
    }
}