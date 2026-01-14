using HRSystem.Application.Common.Interfaces;
using HRSystem.Application.Employees.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRSystem.Application.Employees.Commands.UpdateEmployee;

public class UpdateEmployeeCommandHandler : IRequestHandler<UpdateEmployeeCommand, EmployeeDto>
{
    private readonly IApplicationDbContext _context;

    public UpdateEmployeeCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<EmployeeDto> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employee = await _context.Employees
            .Include(e => e.Organization)
            .Include(e => e.Position)
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Employee with ID {request.Id} not found.");

        employee.FirstName = request.FirstName;
        employee.LastName = request.LastName;
        employee.Email = request.Email;
        employee.Phone = request.Phone;
        employee.OrganizationId = request.OrganizationId;
        employee.PositionId = request.PositionId;
        employee.IsAdmin = request.IsAdmin;
        employee.IsActive = request.IsActive;

        await _context.SaveChangesAsync(cancellationToken);

        return new EmployeeDto
        {
            Id = employee.Id,
            RegistryNo = employee.RegistryNo,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Email = employee.Email,
            Phone = employee.Phone,
            OrganizationId = employee.OrganizationId,
            OrganizationName = employee.Organization.Name,
            PositionId = employee.PositionId,
            PositionName = employee.Position.Name,
            IsActive = employee.IsActive,
            IsAdmin = employee.IsAdmin,
            MustChangePassword = employee.MustChangePassword,
            CreatedAt = employee.CreatedAt,
            UpdatedAt = employee.UpdatedAt
        };
    }
}