namespace HRSystem.Application.Employees.Common;

public class EmployeeListDto
{
    public int Id { get; set; }
    public string RegistryNo { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string OrganizationName { get; set; } = string.Empty;
    public string PositionName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public bool IsAdmin { get; set; }
}