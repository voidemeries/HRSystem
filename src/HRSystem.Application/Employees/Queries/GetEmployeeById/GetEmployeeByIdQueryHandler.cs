using HRSystem.Application.Common.Interfaces;
using HRSystem.Application.Employees.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRSystem.Application.Employees.Queries.GetEmployeeById;

public class GetEmployeeByIdQueryHandler : IRequestHandler<GetEmployeeByIdQuery, EmployeeDto>
{
    private readonly IApplicationDbContext _context;

    public GetEmployeeByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<EmployeeDto> Handle(GetEmployeeByIdQuery request, CancellationToken cancellationToken)
    {
        var employee = await _context.Employees
            .Include(e => e.Organization)
            .Include(e => e.Position)
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Employee with ID {request.Id} not found.");

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