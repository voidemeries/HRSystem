using HRSystem.Domain.Enums;

namespace HRSystem.Application.Common.Interfaces;

public interface IPermissionService
{
    Task<List<PermissionType>> GetUserPermissionsForScreenAsync(int employeeId, int screenResourceId, CancellationToken cancellationToken = default);
    Task<Dictionary<int, List<PermissionType>>> GetUserPermissionsForAllScreensAsync(int employeeId, CancellationToken cancellationToken = default);
    Task<List<int>> GetAuthorizedScreenIdsAsync(int employeeId, CancellationToken cancellationToken = default);
}