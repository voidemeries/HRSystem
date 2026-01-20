namespace HRSystem.Domain.Enums;

/// <summary>
/// Defines the types of permissions that can be assigned to screen resources
/// </summary>
public enum PermissionType
{
    /// <summary>
    /// Permission to view/read the screen resource
    /// </summary>
    View = 1,
    
    /// <summary>
    /// Permission to create, update, and delete data within the screen resource
    /// </summary>
    Manage = 2,
    
    /// <summary>
    /// Permission to approve requests or perform administrative actions
    /// </summary>
    Approve = 3
}