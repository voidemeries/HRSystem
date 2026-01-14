using HRSystem.Application.Authentication.Common;
using HRSystem.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRSystem.Application.Authentication.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponseDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordService _passwordService;
    private readonly ITokenService _tokenService;

    public LoginCommandHandler(
        IApplicationDbContext context,
        IPasswordService passwordService,
        ITokenService tokenService)
    {
        _context = context;
        _passwordService = passwordService;
        _tokenService = tokenService;
    }

    public async Task<LoginResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var employee = await _context.Employees
            .Include(e => e.Organization)
            .Include(e => e.Position)
            .FirstOrDefaultAsync(e => e.RegistryNo == request.RegistryNo, cancellationToken);

        if (employee == null || !employee.IsActive)
        {
            throw new UnauthorizedAccessException("Invalid credentials.");
        }

        if (!_passwordService.VerifyPassword(employee.PasswordHash, request.Password))
        {
            throw new UnauthorizedAccessException("Invalid credentials.");
        }

        var token = _tokenService.GenerateToken(employee);

        return new LoginResponseDto
        {
            Token = token,
            EmployeeId = employee.Id,
            RegistryNo = employee.RegistryNo,
            FullName = $"{employee.FirstName} {employee.LastName}",
            IsAdmin = employee.IsAdmin,
            MustChangePassword = employee.MustChangePassword
        };
    }
}