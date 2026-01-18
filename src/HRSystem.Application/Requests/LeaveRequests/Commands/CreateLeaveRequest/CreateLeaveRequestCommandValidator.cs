using FluentValidation;
using HRSystem.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRSystem.Application.Requests.LeaveRequests.Commands.CreateLeaveRequest;

public class CreateLeaveRequestCommandValidator : AbstractValidator<CreateLeaveRequestCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    public CreateLeaveRequestCommandValidator(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;

        RuleFor(v => v.LeaveType)
            .NotEmpty().WithMessage("Leave type is required.")
            .MaximumLength(100).WithMessage("Leave type must not exceed 100 characters.");

        RuleFor(v => v.StartDate)
            .NotEmpty().WithMessage("Start date is required.")
            .LessThanOrEqualTo(v => v.EndDate).WithMessage("Start date must be before or equal to end date.");

        RuleFor(v => v.EndDate)
            .NotEmpty().WithMessage("End date is required.");

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

        // Only admins can create on behalf of others
        return _currentUserService.IsAdmin;
    }

    private async Task<bool> EmployeeExists(int? employeeId, CancellationToken cancellationToken)
    {
        if (!employeeId.HasValue)
            return true;

        return await _context.Employees.AnyAsync(e => e.Id == employeeId.Value && e.IsActive, cancellationToken);
    }
}