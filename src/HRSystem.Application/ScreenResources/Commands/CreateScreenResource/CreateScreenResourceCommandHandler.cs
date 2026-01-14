using HRSystem.Application.Common.Interfaces;
using HRSystem.Application.ScreenResources.Common;
using HRSystem.Domain.Entities;
using MediatR;

namespace HRSystem.Application.ScreenResources.Commands.CreateScreenResource;

public class CreateScreenResourceCommandHandler : IRequestHandler<CreateScreenResourceCommand, ScreenResourceDto>
{
    private readonly IApplicationDbContext _context;

    public CreateScreenResourceCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ScreenResourceDto> Handle(CreateScreenResourceCommand request, CancellationToken cancellationToken)
    {
        var screenResource = new ScreenResource
        {
            Name = request.Name,
            RoutePath = request.RoutePath,
            ParentScreenId = request.ParentScreenId,
            IsActive = request.IsActive,
            SortOrder = request.SortOrder,
            Icon = request.Icon
        };

        _context.ScreenResources.Add(screenResource);
        await _context.SaveChangesAsync(cancellationToken);

        return new ScreenResourceDto
        {
            Id = screenResource.Id,
            Name = screenResource.Name,
            RoutePath = screenResource.RoutePath,
            ParentScreenId = screenResource.ParentScreenId,
            IsActive = screenResource.IsActive,
            SortOrder = screenResource.SortOrder,
            Icon = screenResource.Icon,
            CreatedAt = screenResource.CreatedAt,
            UpdatedAt = screenResource.UpdatedAt
        };
    }
}