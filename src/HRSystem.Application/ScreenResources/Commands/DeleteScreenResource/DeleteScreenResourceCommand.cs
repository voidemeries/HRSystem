using MediatR;

namespace HRSystem.Application.ScreenResources.Commands.DeleteScreenResource;

public record DeleteScreenResourceCommand(int Id) : IRequest<Unit>;