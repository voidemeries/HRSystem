namespace HRSystem.Application.Common.Interfaces;

public interface IApproverService
{
    Task<int?> GetApproverPositionIdAsync(int employeeId, CancellationToken cancellationToken = default);
    Task<int?> GetDesignatedApproverIdAsync(int approverPositionId, CancellationToken cancellationToken = default);
}