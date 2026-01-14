using HRSystem.Application.Common.Interfaces;
using HRSystem.Application.Employees.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRSystem.Application.Employees.Queries.GetEmployees;

public class GetEmployeesQueryHandler : IRequestHandler<GetEmployeesQuery, List<EmployeeListDto>>
{
    private readonly IApplicationDbContext _context;

    public GetEmployeesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<EmployeeListDto>> Handle(GetEmployeesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Employees
            .Include(e => e.Organization)
            .Include(e => e.Position)
            .AsQueryable();

        if (request.OrganizationId.HasValue)
        {
            query = query.Where(e => e.OrganizationId == request.OrganizationId.Value);
        }

        if (request.PositionId.HasValue)
        {
            query = query.Where(e => e.PositionId == request.PositionId.Value);
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(e => e.IsActive == request.IsActive.Value);
        }

        if (request.IsAdmin.HasValue)
        {
            query = query.Where(e => e.IsAdmin == request.IsAdmin.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            query = query.Where(e =>
                e.RegistryNo.ToLower().Contains(searchTerm) ||
                e.FirstName.ToLower().Contains(searchTerm) ||
                e.LastName.ToLower().Contains(searchTerm) ||
                (e.Email != null && e.Email.ToLower().Contains(searchTerm)));
        }

        return await query
            .OrderBy(e => e.RegistryNo)
            .Select(e => new EmployeeListDto
            {
                Id = e.Id,
                RegistryNo = e.RegistryNo,
                FirstName = e.FirstName,
                LastName = e.LastName,
                Email = e.Email,
                OrganizationName = e.Organization.Name,
                PositionName = e.Position.Name,
                IsActive = e.IsActive,
                IsAdmin = e.IsAdmin
            })
            .ToListAsync(cancellationToken);
    }
}