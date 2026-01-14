using HRSystem.Application.Employees.Common;
using MediatR;

namespace HRSystem.Application.Employees.Commands.CreateEmployee;

public record CreateEmployeeCommand : IRequest<CreateEmployeeResponseDto>
{
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string? Email { get; init; }
    public string? Phone { get; init; }
    public int OrganizationId { get; init; }
    public int PositionId { get; init; }
    public bool IsAdmin { get; init; }
}