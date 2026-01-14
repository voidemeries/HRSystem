using HRSystem.Application.Permissions.Common;
using MediatR;

namespace HRSystem.Application.Permissions.Queries.SearchUserScreens;

public record SearchUserScreensQuery : IRequest<List<UserScreenSearchResultDto>>
{
    public string Query { get; init; } = string.Empty;
}