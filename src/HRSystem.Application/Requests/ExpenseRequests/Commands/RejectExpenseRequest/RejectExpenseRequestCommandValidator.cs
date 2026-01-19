namespace HRSystem.Application.Requests.ExpenseRequests.Commands.RejectExpenseRequest;

using FluentValidation;

public class RejectExpenseRequestCommandValidator : AbstractValidator<RejectExpenseRequestCommand>
{
    public RejectExpenseRequestCommandValidator()
    {
        RuleFor(v => v.RejectionReason)
            .NotEmpty().WithMessage("Rejection reason is required.")
            .MaximumLength(1000).WithMessage("Rejection reason must not exceed 1000 characters.");
    }
}