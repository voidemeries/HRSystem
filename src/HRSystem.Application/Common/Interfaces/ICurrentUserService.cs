namespace HRSystem.Application.Common.Interfaces;

public interface ICurrentUserService
{
    int? EmployeeId { get; }
    string? RegistryNo { get; }
    bool IsAdmin { get; }
    int? OrganizationId { get; }
    int? PositionId { get; }
}