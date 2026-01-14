using HRSystem.Domain.Common;

namespace HRSystem.Domain.Entities;

public class Employee : BaseEntity
{
    public string RegistryNo { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string PasswordHash { get; set; } = string.Empty;
    public bool MustChangePassword { get; set; } = true;
    public bool IsActive { get; set; } = true;
    public bool IsAdmin { get; set; } = false;
    public int OrganizationId { get; set; }
    public int PositionId { get; set; }

    public Organization Organization { get; set; } = null!;
    public Position Position { get; set; } = null!;
}