using HRSystem.Application.Permissions.Common;
using HRSystem.Domain.Enums;
using MediatR;

namespace HRSystem.Application.Permissions.Commands.CreatePermissionAssignment;

public record CreatePermissionAssignmentCommand : IRequest<PermissionAssignmentDto>
{
    public int ScreenResourceId { get; init; }
    public ScopeType ScopeType { get; init; }
    public int ScopeId { get; init; }
    public PermissionType PermissionType { get; init; }
}