namespace HRSystem.Application.Employees.Common;

public class CreateEmployeeResponseDto
{
    public int Id { get; set; }
    public string RegistryNo { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public int OrganizationId { get; set; }
    public int PositionId { get; set; }
    public bool IsActive { get; set; }
    public bool IsAdmin { get; set; }
    public bool MustChangePassword { get; set; }
    public DateTime CreatedAt { get; set; }
    public string InitialPassword { get; set; } = string.Empty;
}