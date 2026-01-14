using HRSystem.Application.Permissions.Common;
using MediatR;

namespace HRSystem.Application.Permissions.Queries.GetPermissionAssignmentById;

public record GetPermissionAssignmentByIdQuery(int Id) : IRequest<PermissionAssignmentDto>;