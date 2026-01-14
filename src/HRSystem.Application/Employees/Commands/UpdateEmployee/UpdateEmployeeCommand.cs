using HRSystem.Application.Employees.Common;
using MediatR;

namespace HRSystem.Application.Employees.Commands.UpdateEmployee;

public record UpdateEmployeeCommand : IRequest<EmployeeDto>
{
    public int Id { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string? Email { get; init; }
    public string? Phone { get; init; }
    public int OrganizationId { get; init; }
    public int PositionId { get; init; }
    public bool IsAdmin { get; init; }
    public bool IsActive { get; init; }
}