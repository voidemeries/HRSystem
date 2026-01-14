using HRSystem.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRSystem.Application.Authentication.Commands.ChangePassword;

public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Unit>
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordService _passwordService;
    private readonly ICurrentUserService _currentUserService;

    public ChangePasswordCommandHandler(
        IApplicationDbContext context,
        IPasswordService passwordService,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _passwordService = passwordService;
        _currentUserService = currentUserService;
    }

    public async Task<Unit> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var employeeId = _currentUserService.EmployeeId
            ?? throw new UnauthorizedAccessException("User not authenticated.");

        var employee = await _context.Employees
            .FirstOrDefaultAsync(e => e.Id == employeeId, cancellationToken)
            ?? throw new UnauthorizedAccessException("Employee not found.");

        if (!_passwordService.VerifyPassword(employee.PasswordHash, request.OldPassword))
        {
            throw new UnauthorizedAccessException("Old password is incorrect.");
        }

        employee.PasswordHash = _passwordService.HashPassword(request.NewPassword);
        employee.MustChangePassword = false;

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}