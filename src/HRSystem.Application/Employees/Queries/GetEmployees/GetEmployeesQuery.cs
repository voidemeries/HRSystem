using HRSystem.Application.Employees.Common;
using MediatR;

namespace HRSystem.Application.Employees.Queries.GetEmployees;

public record GetEmployeesQuery : IRequest<List<EmployeeListDto>>
{
    public int? OrganizationId { get; init; }
    public int? PositionId { get; init; }
    public bool? IsActive { get; init; }
    public bool? IsAdmin { get; init; }
    public string? SearchTerm { get; init; }
}