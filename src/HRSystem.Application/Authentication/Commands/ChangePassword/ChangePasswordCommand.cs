using MediatR;

namespace HRSystem.Application.Authentication.Commands.ChangePassword;

public record ChangePasswordCommand : IRequest<Unit>
{
    public string OldPassword { get; init; } = string.Empty;
    public string NewPassword { get; init; } = string.Empty;
}