using HRSystem.Application.Authentication.Common;
using MediatR;

namespace HRSystem.Application.Authentication.Commands.Login;

public record LoginCommand : IRequest<LoginResponseDto>
{
    public string RegistryNo { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}