using HRSystem.Application.Common.Interfaces;
using HRSystem.Application.ScreenResources.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRSystem.Application.ScreenResources.Queries.GetScreenResourceById;

public class GetScreenResourceByIdQueryHandler : IRequestHandler<GetScreenResourceByIdQuery, ScreenResourceDto>
{
    private readonly IApplicationDbContext _context;

    public GetScreenResourceByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ScreenResourceDto> Handle(GetScreenResourceByIdQuery request, CancellationToken cancellationToken)
    {
        var screenResource = await _context.ScreenResources
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Screen resource with ID {request.Id} not found.");

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