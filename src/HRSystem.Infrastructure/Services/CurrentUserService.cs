using System.Security.Claims;
using HRSystem.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace HRSystem.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int? EmployeeId
    {
        get
        {
            var employeeIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirstValue("EmployeeId");
            return int.TryParse(employeeIdClaim, out var id) ? id : null;
        }
    }

    public string? RegistryNo => _httpContextAccessor.HttpContext?.User?.FindFirstValue("RegistryNo");

    public bool IsAdmin
    {
        get
        {
            var isAdminClaim = _httpContextAccessor.HttpContext?.User?.FindFirstValue("IsAdmin");
            return bool.TryParse(isAdminClaim, out var isAdmin) && isAdmin;
        }
    }

    public int? OrganizationId
    {
        get
        {
            var orgIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirstValue("OrganizationId");
            return int.TryParse(orgIdClaim, out var id) ? id : null;
        }
    }

    public int? PositionId
    {
        get
        {
            var posIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirstValue("PositionId");
            return int.TryParse(posIdClaim, out var id) ? id : null;
        }
    }
}