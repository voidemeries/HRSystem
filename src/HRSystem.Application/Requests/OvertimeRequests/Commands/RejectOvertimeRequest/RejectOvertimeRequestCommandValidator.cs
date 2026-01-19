namespace HRSystem.Application.Requests.OvertimeRequests.Commands.RejectOvertimeRequest;

using FluentValidation;

public class RejectOvertimeRequestCommandValidator : AbstractValidator<RejectOvertimeRequestCommand>
{
    public RejectOvertimeRequestCommandValidator()
    {
        RuleFor(v => v.RejectionReason)
            .NotEmpty().WithMessage("Rejection reason is required.")
            .MaximumLength(1000).WithMessage("Rejection reason must not exceed 1000 characters.");
    }
}