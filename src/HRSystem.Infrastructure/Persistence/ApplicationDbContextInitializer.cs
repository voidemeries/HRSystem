using HRSystem.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HRSystem.Infrastructure.Persistence;

public class ApplicationDbContextInitializer
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ApplicationDbContextInitializer> _logger;

    public ApplicationDbContextInitializer(
        ApplicationDbContext context,
        ILogger<ApplicationDbContextInitializer> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task InitializeAsync()
    {
        try
        {
            // If there are migrations, apply them; otherwise create schema from model
            var hasMigrations = _context.Database.GetMigrations().Any();
            if (hasMigrations)
            {
                await _context.Database.MigrateAsync();
            }
            else
            {
                await _context.Database.EnsureCreatedAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while migrating the database.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    private async Task TrySeedAsync()
    {
        // Seed Organizations
        if (!await _context.Organizations.AnyAsync())
        {
            var rootOrg = new Organization
            {
                Name = "Head Office",
                Code = "HQ",
                CreatedAt = DateTime.UtcNow
            };

            _context.Organizations.Add(rootOrg);
            await _context.SaveChangesAsync();

            var hrDept = new Organization
            {
                Name = "HR Department",
                Code = "HR",
                ParentOrganizationId = rootOrg.Id,
                CreatedAt = DateTime.UtcNow
            };

            var itDept = new Organization
            {
                Name = "IT Department",
                Code = "IT",
                ParentOrganizationId = rootOrg.Id,
                CreatedAt = DateTime.UtcNow
            };

            _context.Organizations.AddRange(hrDept, itDept);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Seeded organizations");
        }

        // Seed Positions
        if (!await _context.Positions.AnyAsync())
        {
            var ceo = new Position
            {
                Name = "CEO",
                Code = "CEO",
                CreatedAt = DateTime.UtcNow
            };

            _context.Positions.Add(ceo);
            await _context.SaveChangesAsync();

            var hrManager = new Position
            {
                Name = "HR Manager",
                Code = "HRM",
                ParentPositionId = ceo.Id,
                CreatedAt = DateTime.UtcNow
            };

            var itManager = new Position
            {
                Name = "IT Manager",
                Code = "ITM",
                ParentPositionId = ceo.Id,
                CreatedAt = DateTime.UtcNow
            };

            _context.Positions.AddRange(hrManager, itManager);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Seeded positions");
        }

        // Seed Admin Employee
        if (!await _context.Employees.AnyAsync())
        {
            var rootOrg = await _context.Organizations.FirstAsync(o => o.Code == "HQ");
            var ceoPosition = await _context.Positions.FirstAsync(p => p.Code == "CEO");

            var passwordHasher = new PasswordHasher<Employee>();
            var admin = new Employee
            {
                RegistryNo = "ADMIN001",
                FirstName = "System",
                LastName = "Administrator",
                Email = "admin@hrsystem.com",
                IsActive = true,
                IsAdmin = true,
                MustChangePassword = false,
                OrganizationId = rootOrg.Id,
                PositionId = ceoPosition.Id,
                CreatedAt = DateTime.UtcNow
            };

            admin.PasswordHash = passwordHasher.HashPassword(admin, "Admin123!");

            _context.Employees.Add(admin);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Seeded admin employee (RegistryNo: ADMIN001, Password: Admin123!)");
        }

        // Seed Screen Resources
        if (!await _context.ScreenResources.AnyAsync())
        {
            var dashboard = new ScreenResource
            {
                Name = "Dashboard",
                RoutePath = "/dashboard",
                IsActive = true,
                SortOrder = 1,
                Icon = "dashboard",
                CreatedAt = DateTime.UtcNow
            };

            _context.ScreenResources.Add(dashboard);
            await _context.SaveChangesAsync();

            var hrModule = new ScreenResource
            {
                Name = "HR Management",
                RoutePath = "/hr",
                IsActive = true,
                SortOrder = 2,
                Icon = "people",
                CreatedAt = DateTime.UtcNow
            };

            _context.ScreenResources.Add(hrModule);
            await _context.SaveChangesAsync();

            var orgScreen = new ScreenResource
            {
                Name = "Organizations",
                RoutePath = "/hr/organizations",
                ParentScreenId = hrModule.Id,
                IsActive = true,
                SortOrder = 1,
                Icon = "corporate_fare",
                CreatedAt = DateTime.UtcNow
            };

            var posScreen = new ScreenResource
            {
                Name = "Positions",
                RoutePath = "/hr/positions",
                ParentScreenId = hrModule.Id,
                IsActive = true,
                SortOrder = 2,
                Icon = "work",
                CreatedAt = DateTime.UtcNow
            };

            var empScreen = new ScreenResource
            {
                Name = "Employees",
                RoutePath = "/hr/employees",
                ParentScreenId = hrModule.Id,
                IsActive = true,
                SortOrder = 3,
                Icon = "badge",
                CreatedAt = DateTime.UtcNow
            };

            _context.ScreenResources.AddRange(orgScreen, posScreen, empScreen);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Seeded screen resources");
        }

        if (!await _context.PermissionAssignments.AnyAsync())
        {
            var hrDept = await _context.Organizations.FirstAsync(o => o.Code == "HR");
            var hrManager = await _context.Positions.FirstAsync(p => p.Code == "HRM");
            var dashboard = await _context.ScreenResources.FirstAsync(s => s.RoutePath == "/dashboard");
            var hrModule = await _context.ScreenResources.FirstAsync(s => s.RoutePath == "/hr");

            var permissions = new List<PermissionAssignment>
            {
                 // Give HR Department View access to Dashboard
                 new PermissionAssignment
                 {
                     ScreenResourceId = dashboard.Id,
                     ScopeType = Domain.Enums.ScopeType.Organization,
                     ScopeId = hrDept.Id,
                     PermissionType = Domain.Enums.PermissionType.View,
                     CreatedAt = DateTime.UtcNow
                 },
                 // Give HR Manager Position View and Manage access to HR Module
                 new PermissionAssignment
                 {
                     ScreenResourceId = hrModule.Id,
                     ScopeType = Domain.Enums.ScopeType.Position,
                     ScopeId = hrManager.Id,
                     PermissionType = Domain.Enums.PermissionType.View,
                     CreatedAt = DateTime.UtcNow
                 },
                 new PermissionAssignment
                 {
                     ScreenResourceId = hrModule.Id,
                     ScopeType = Domain.Enums.ScopeType.Position,
                     ScopeId = hrManager.Id,
                     PermissionType = Domain.Enums.PermissionType.Manage,
                     CreatedAt = DateTime.UtcNow
                 }
            };

            _context.PermissionAssignments.AddRange(permissions);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Seeded permission assignments");
        }
    }
}
