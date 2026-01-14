using MediatR;

namespace HRSystem.Application.Organizations.Commands.DeleteOrganization;

public record DeleteOrganizationCommand(int Id) : IRequest<Unit>;