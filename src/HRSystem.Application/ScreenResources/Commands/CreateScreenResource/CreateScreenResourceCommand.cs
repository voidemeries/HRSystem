using HRSystem.Application.ScreenResources.Common;
using MediatR;

namespace HRSystem.Application.ScreenResources.Commands.CreateScreenResource;

public record CreateScreenResourceCommand : IRequest<ScreenResourceDto>
{
    public string Name { get; init; } = string.Empty;
    public string RoutePath { get; init; } = string.Empty;
    public int? ParentScreenId { get; init; }
    public bool IsActive { get; init; } = true;
    public int SortOrder { get; init; }
    public string? Icon { get; init; }
}