namespace HRSystem.Application.Authentication.Common;

public class CurrentUserDto
{
    public int EmployeeId { get; set; }
    public string RegistryNo { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public int OrganizationId { get; set; }
    public string OrganizationName { get; set; } = string.Empty;
    public int PositionId { get; set; }
    public string PositionName { get; set; } = string.Empty;
    public bool IsAdmin { get; set; }
    public bool MustChangePassword { get; set; }
}