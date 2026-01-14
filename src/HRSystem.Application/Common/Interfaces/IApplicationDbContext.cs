using HRSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HRSystem.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Organization> Organizations { get; }
    DbSet<Position> Positions { get; }
    DbSet<Employee> Employees { get; }
    DbSet<ScreenResource> ScreenResources { get; }
    DbSet<PermissionAssignment> PermissionAssignments { get; }
    DbSet<LeaveRequest> LeaveRequests { get; }
    DbSet<RemoteWorkRequest> RemoteWorkRequests { get; }
    DbSet<TravelRequest> TravelRequests { get; }
    DbSet<ExpenseRequest> ExpenseRequests { get; }
    DbSet<OvertimeRequest> OvertimeRequests { get; }
    DbSet<TrainingSupportRequest> TrainingSupportRequests { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}