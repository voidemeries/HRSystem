using HRSystem.Application.Authentication.Common;
using MediatR;

namespace HRSystem.Application.Authentication.Queries.GetCurrentUser;

public record GetCurrentUserQuery : IRequest<CurrentUserDto>;