using HRSystem.Domain.Common;
using HRSystem.Domain.Enums;

namespace HRSystem.Domain.Entities;

public class PermissionAssignment : BaseEntity
{
    public int ScreenResourceId { get; set; }
    public ScopeType ScopeType { get; set; }
    public int ScopeId { get; set; }
    public PermissionType PermissionType { get; set; }

    public ScreenResource ScreenResource { get; set; } = null!;
}