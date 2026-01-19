namespace HRSystem.Application.Requests.RemoteWorkRequests.Commands.RejectRemoteWorkRequest;

using FluentValidation;

public class RejectRemoteWorkRequestCommandValidator : AbstractValidator<RejectRemoteWorkRequestCommand>
{
    public RejectRemoteWorkRequestCommandValidator()
    {
        RuleFor(v => v.RejectionReason)
            .NotEmpty().WithMessage("Rejection reason is required.")
            .MaximumLength(1000).WithMessage("Rejection reason must not exceed 1000 characters.");
    }
}