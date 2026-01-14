using HRSystem.Application.ScreenResources.Common;
using MediatR;

namespace HRSystem.Application.ScreenResources.Queries.SearchScreenResources;

public record SearchScreenResourcesQuery : IRequest<List<ScreenResourceSearchResultDto>>
{
    public string Query { get; init; } = string.Empty;
}