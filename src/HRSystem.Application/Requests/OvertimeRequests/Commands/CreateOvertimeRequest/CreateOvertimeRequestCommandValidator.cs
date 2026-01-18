using FluentValidation;
using HRSystem.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRSystem.Application.Requests.OvertimeRequests.Commands.CreateOvertimeRequest;

public class CreateOvertimeRequestCommandValidator : AbstractValidator<CreateOvertimeRequestCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public CreateOvertimeRequestCommandValidator(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;

        RuleFor(v => v.OvertimeDate)
            .NotEmpty().WithMessage("Overtime date is required.");

        RuleFor(v => v.Hours)
            .GreaterThan(0).WithMessage("Hours must be greater than zero.")
            .LessThanOrEqualTo(24).WithMessage("Hours cannot exceed 24 in a single day.");

        RuleFor(v => v.Reason)
            .NotEmpty().WithMessage("Reason is required.")
            .MaximumLength(1000).WithMessage("Reason must not exceed 1000 characters.");

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