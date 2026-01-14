namespace HRSystem.Application.Authentication.Common;

public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public int EmployeeId { get; set; }
    public string RegistryNo { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public bool IsAdmin { get; set; }
    public bool MustChangePassword { get; set; }
}