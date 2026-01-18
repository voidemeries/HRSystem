using FluentValidation;
using HRSystem.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRSystem.Application.Requests.TrainingSupportRequests.Commands.CreateTrainingSupportRequest;

public class CreateTrainingSupportRequestCommandValidator : AbstractValidator<CreateTrainingSupportRequestCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public CreateTrainingSupportRequestCommandValidator(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;

        RuleFor(v => v.TrainingName)
            .NotEmpty().WithMessage("Training name is required.")
            .MaximumLength(200).WithMessage("Training name must not exceed 200 characters.");

        RuleFor(v => v.Provider)
            .NotEmpty().WithMessage("Provider is required.")
            .MaximumLength(200).WithMessage("Provider must not exceed 200 characters.");

        RuleFor(v => v.StartDate)
            .NotEmpty().WithMessage("Start date is required.")
            .LessThanOrEqualTo(v => v.EndDate).WithMessage("Start date must be before or equal to end date.");

        RuleFor(v => v.EndDate)
            .NotEmpty().WithMessage("End date is required.");

        RuleFor(v => v.Cost)
            .GreaterThanOrEqualTo(0).WithMessage("Cost must be non-negative.");

        RuleFor(v => v.Justification)
            .NotEmpty().WithMessage("Justification is required.")
            .MaximumLength(1000).WithMessage("Justification must not exceed 1000 characters.");

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