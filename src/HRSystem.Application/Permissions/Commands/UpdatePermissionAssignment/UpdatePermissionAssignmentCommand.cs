using HRSystem.Application.Permissions.Common;
using HRSystem.Domain.Enums;
using MediatR;

namespace HRSystem.Application.Permissions.Commands.UpdatePermissionAssignment;

public record UpdatePermissionAssignmentCommand : IRequest<PermissionAssignmentDto>
{
    public int Id { get; init; }
    public int ScreenResourceId { get; init; }
    public ScopeType ScopeType { get; init; }
    public int ScopeId { get; init; }
    public PermissionType PermissionType { get; init; }
}