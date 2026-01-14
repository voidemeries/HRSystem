using HRSystem.Application.Common.Interfaces;
using HRSystem.Application.ScreenResources.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRSystem.Application.ScreenResources.Commands.UpdateScreenResource;

public class UpdateScreenResourceCommandHandler : IRequestHandler<UpdateScreenResourceCommand, ScreenResourceDto>
{
    private readonly IApplicationDbContext _context;

    public UpdateScreenResourceCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ScreenResourceDto> Handle(UpdateScreenResourceCommand request, CancellationToken cancellationToken)
    {
        var screenResource = await _context.ScreenResources
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Screen resource with ID {request.Id} not found.");

        screenResource.Name = request.Name;
        screenResource.RoutePath = request.RoutePath;
        screenResource.ParentScreenId = request.ParentScreenId;
        screenResource.IsActive = request.IsActive;
        screenResource.SortOrder = request.SortOrder;
        screenResource.Icon = request.Icon;

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