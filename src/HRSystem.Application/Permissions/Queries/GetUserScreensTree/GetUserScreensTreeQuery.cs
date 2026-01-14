using HRSystem.Application.Permissions.Common;
using MediatR;

namespace HRSystem.Application.Permissions.Queries.GetUserScreensTree;

public record GetUserScreensTreeQuery : IRequest<List<UserScreenTreeDto>>;