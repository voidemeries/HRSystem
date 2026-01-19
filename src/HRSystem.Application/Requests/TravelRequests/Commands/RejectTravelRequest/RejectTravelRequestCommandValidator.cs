namespace HRSystem.Application.Requests.TravelRequests.Commands.RejectTravelRequest;

using FluentValidation;

public class RejectTravelRequestCommandValidator : AbstractValidator<RejectTravelRequestCommand>
{
    public RejectTravelRequestCommandValidator()
    {
        RuleFor(v => v.RejectionReason)
            .NotEmpty().WithMessage("Rejection reason is required.")
            .MaximumLength(1000).WithMessage("Rejection reason must not exceed 1000 characters.");
    }
}