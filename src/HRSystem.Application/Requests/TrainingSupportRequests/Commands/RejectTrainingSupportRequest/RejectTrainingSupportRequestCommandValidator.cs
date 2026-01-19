namespace HRSystem.Application.Requests.TrainingSupportRequests.Commands.RejectTrainingSupportRequest;

using FluentValidation;

public class RejectTrainingSupportRequestCommandValidator : AbstractValidator<RejectTrainingSupportRequestCommand>
{
    public RejectTrainingSupportRequestCommandValidator()
    {
        RuleFor(v => v.RejectionReason)
            .NotEmpty().WithMessage("Rejection reason is required.")
            .MaximumLength(1000).WithMessage("Rejection reason must not exceed 1000 characters.");
    }
}