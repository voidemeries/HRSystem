using HRSystem.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRSystem.Infrastructure.Services;

/// <summary>
/// Service for resolving approvers based on position hierarchy.
/// Approver Resolution Logic:
/// 1. Find the parent position of the employee's position
/// 2. If multiple employees exist in that parent position, select the one with the lowest EmployeeId (deterministic)
/// </summary>
public class ApproverService : IApproverService
{
    private readonly IApplicationDbContext _context;

    public ApproverService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int?> GetApproverPositionIdAsync(int employeeId, CancellationToken cancellationToken = default)
    {
        var employee = await _context.Employees
            .Include(e => e.Position)
            .FirstOrDefaultAsync(e => e.Id == employeeId, cancellationToken);

        if (employee == null || employee.Position == null)
            return null;

        // Return the parent position ID (approver's position)
        return employee.Position.ParentPositionId;
    }

    public async Task<int?> GetDesignatedApproverIdAsync(int approverPositionId, CancellationToken cancellationToken = default)
    {
        // Find all active employees in the approver position
        // If multiple exist, select the one with the lowest EmployeeId (deterministic selection)
        var approver = await _context.Employees
            .Where(e => e.PositionId == approverPositionId && e.IsActive)
            .OrderBy(e => e.Id)
            .FirstOrDefaultAsync(cancellationToken);

        return approver?.Id;
    }
}