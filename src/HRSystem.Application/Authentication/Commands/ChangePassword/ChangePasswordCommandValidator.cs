using FluentValidation;

namespace HRSystem.Application.Authentication.Commands.ChangePassword;

public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(v => v.OldPassword)
            .NotEmpty().WithMessage("Old password is required.");

        RuleFor(v => v.NewPassword)
            .NotEmpty().WithMessage("New password is required.")
            .MinimumLength(8).WithMessage("New password must be at least 8 characters long.")
            .NotEqual(v => v.OldPassword).WithMessage("New password must be different from old password.");
    }
}