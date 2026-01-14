using HRSystem.Application.Authentication.Common;
using HRSystem.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRSystem.Application.Authentication.Queries.GetCurrentUser;

public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, CurrentUserDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetCurrentUserQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<CurrentUserDto> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        var employeeId = _currentUserService.EmployeeId
            ?? throw new UnauthorizedAccessException("User not authenticated.");

        var employee = await _context.Employees
            .Include(e => e.Organization)
            .Include(e => e.Position)
            .FirstOrDefaultAsync(e => e.Id == employeeId, cancellationToken)
            ?? throw new UnauthorizedAccessException("Employee not found.");

        return new CurrentUserDto
        {
            EmployeeId = employee.Id,
            RegistryNo = employee.RegistryNo,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Email = employee.Email,
            OrganizationId = employee.OrganizationId,
            OrganizationName = employee.Organization.Name,
            PositionId = employee.PositionId,
            PositionName = employee.Position.Name,
            IsAdmin = employee.IsAdmin,
            MustChangePassword = employee.MustChangePassword
        };
    }
}