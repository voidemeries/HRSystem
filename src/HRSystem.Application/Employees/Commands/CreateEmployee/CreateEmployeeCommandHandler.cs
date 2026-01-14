using HRSystem.Application.Common.Interfaces;
using HRSystem.Application.Employees.Common;
using HRSystem.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HRSystem.Application.Employees.Commands.CreateEmployee;

public class CreateEmployeeCommandHandler : IRequestHandler<CreateEmployeeCommand, CreateEmployeeResponseDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordService _passwordService;

    public CreateEmployeeCommandHandler(
        IApplicationDbContext context,
        IPasswordService passwordService)
    {
        _context = context;
        _passwordService = passwordService;
    }

    public async Task<CreateEmployeeResponseDto> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
    {
        // Generate unique RegistryNo
        var registryNo = await GenerateUniqueRegistryNo(cancellationToken);

        // Generate initial password
        var initialPassword = _passwordService.GenerateRandomPassword();

        var employee = new Employee
        {
            RegistryNo = registryNo,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Phone = request.Phone,
            OrganizationId = request.OrganizationId,
            PositionId = request.PositionId,
            IsAdmin = request.IsAdmin,
            IsActive = true,
            MustChangePassword = true,
            PasswordHash = _passwordService.HashPassword(initialPassword)
        };

        _context.Employees.Add(employee);
        await _context.SaveChangesAsync(cancellationToken);

        return new CreateEmployeeResponseDto
        {
            Id = employee.Id,
            RegistryNo = employee.RegistryNo,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Email = employee.Email,
            Phone = employee.Phone,
            OrganizationId = employee.OrganizationId,
            PositionId = employee.PositionId,
            IsActive = employee.IsActive,
            IsAdmin = employee.IsAdmin,
            MustChangePassword = employee.MustChangePassword,
            CreatedAt = employee.CreatedAt,
            InitialPassword = initialPassword
        };
    }

    private async Task<string> GenerateUniqueRegistryNo(CancellationToken cancellationToken)
    {
        var lastEmployee = await _context.Employees
            .Where(e => e.RegistryNo.StartsWith("EMP"))
            .OrderByDescending(e => e.Id)
            .FirstOrDefaultAsync(cancellationToken);

        int nextNumber = 1;

        if (lastEmployee != null && lastEmployee.RegistryNo.Length > 3)
        {
            var numberPart = lastEmployee.RegistryNo.Substring(3);
            if (int.TryParse(numberPart, out int lastNumber))
            {
                nextNumber = lastNumber + 1;
            }
        }

        var registryNo = $"EMP{nextNumber:D6}";

        // Ensure uniqueness (safety check)
        while (await _context.Employees.AnyAsync(e => e.RegistryNo == registryNo, cancellationToken))
        {
            nextNumber++;
            registryNo = $"EMP{nextNumber:D6}";
        }

        return registryNo;
    }
}