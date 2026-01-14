using HRSystem.Application.ScreenResources.Common;
using MediatR;

namespace HRSystem.Application.ScreenResources.Queries.GetScreenResourcesTree;

public record GetScreenResourcesTreeQuery : IRequest<List<ScreenResourceTreeDto>>;