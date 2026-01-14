using MediatR;

namespace HRSystem.Application.Permissions.Commands.DeletePermissionAssignment;

public record DeletePermissionAssignmentCommand(int Id) : IRequest<Unit>;