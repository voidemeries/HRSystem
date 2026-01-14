using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HRSystem.Application.Common.Interfaces;
using HRSystem.Domain.Entities;
using Microsoft.IdentityModel.Tokens;
using HRSystem.Infrastructure.Persistence.Configurations;

namespace HRSystem.Infrastructure.Services;

public class TokenService : ITokenService
{
    private readonly JwtSettings _jwtSettings;

    public TokenService(JwtSettings jwtSettings)
    {
        _jwtSettings = jwtSettings;
    }

    public string GenerateToken(Employee employee)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, employee.Id.ToString()),
            new Claim("EmployeeId", employee.Id.ToString()),
            new Claim("RegistryNo", employee.RegistryNo),
            new Claim("IsAdmin", employee.IsAdmin.ToString()),
            new Claim("OrganizationId", employee.OrganizationId.ToString()),
            new Claim("PositionId", employee.PositionId.ToString()),
            new Claim(ClaimTypes.Name, $"{employee.FirstName} {employee.LastName}")
        };

        if (!string.IsNullOrWhiteSpace(employee.Email))
        {
            claims.Add(new Claim(ClaimTypes.Email, employee.Email));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(_jwtSettings.ExpiryInHours),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}