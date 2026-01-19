using FluentValidation;
using HRSystem.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRSystem.Application.Employees.Commands.DeleteEmployee;

public class DeleteEmployeeCommandValidator : AbstractValidator<DeleteEmployeeCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteEmployeeCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(v => v.Id)
            .MustAsync(EmployeeExists).WithMessage("Employee does not exist.");
    }

    private async Task<bool> EmployeeExists(int id, CancellationToken cancellationToken)
    {
        return await _context.Employees.AnyAsync(e => e.Id == id, cancellationToken);
    }
}