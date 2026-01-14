using HRSystem.Application.ScreenResources.Common;
using MediatR;

namespace HRSystem.Application.ScreenResources.Queries.GetScreenResourceById;

public record GetScreenResourceByIdQuery(int Id) : IRequest<ScreenResourceDto>;