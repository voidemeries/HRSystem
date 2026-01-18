using FluentValidation;
using HRSystem.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRSystem.Application.Requests.ExpenseRequests.Commands.CreateExpenseRequest;

public class CreateExpenseRequestCommandValidator : AbstractValidator<CreateExpenseRequestCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public CreateExpenseRequestCommandValidator(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;

        RuleFor(v => v.ExpenseDate)
            .NotEmpty().WithMessage("Expense date is required.");

        RuleFor(v => v.Category)
            .NotEmpty().WithMessage("Category is required.")
            .MaximumLength(100).WithMessage("Category must not exceed 100 characters.");

        RuleFor(v => v.Amount)
            .GreaterThan(0).WithMessage("Amount must be greater than zero.");

        RuleFor(v => v.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.");

        RuleFor(v => v.ForEmployeeId)
            .MustAsync(BeAuthorizedToCreateOnBehalf).WithMessage("Only admins can create requests on behalf of others.")
            .MustAsync(EmployeeExists).WithMessage("Employee does not exist.")
            .When(v => v.ForEmployeeId.HasValue);
    }

    private async Task<bool> BeAuthorizedToCreateOnBehalf(int? forEmployeeId, CancellationToken cancellationToken)
    {
        if (!forEmployeeId.HasValue)
            return true;

        return _currentUserService.IsAdmin;
    }

    private async Task<bool> EmployeeExists(int? employeeId, CancellationToken cancellationToken)
    {
        if (!employeeId.HasValue)
            return true;

        return await _context.Employees.AnyAsync(e => e.Id == employeeId.Value && e.IsActive, cancellationToken);
    }
}