using HRSystem.Domain.Entities;

namespace HRSystem.Application.Common.Interfaces;

public interface ITokenService
{
    string GenerateToken(Employee employee);
}