using HRSystem.Application.ScreenResources.Common;
using MediatR;

namespace HRSystem.Application.ScreenResources.Commands.UpdateScreenResource;

public record UpdateScreenResourceCommand : IRequest<ScreenResourceDto>
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string RoutePath { get; init; } = string.Empty;
    public int? ParentScreenId { get; init; }
    public bool IsActive { get; init; }
    public int SortOrder { get; init; }
    public string? Icon { get; init; }
}