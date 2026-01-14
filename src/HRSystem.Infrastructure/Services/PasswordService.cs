using System.Security.Cryptography;
using System.Text;
using HRSystem.Application.Common.Interfaces;
using HRSystem.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace HRSystem.Infrastructure.Services;

public class PasswordService : IPasswordService
{
    private readonly PasswordHasher<Employee> _passwordHasher;

    public PasswordService()
    {
        _passwordHasher = new PasswordHasher<Employee>();
    }

    public string GenerateRandomPassword()
    {
        const string uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string lowercase = "abcdefghijklmnopqrstuvwxyz";
        const string digits = "0123456789";
        const string special = "!@#$%^&*";
        const string allChars = uppercase + lowercase + digits + special;

        var password = new StringBuilder();

        // Ensure at least one character from each category
        password.Append(uppercase[RandomNumberGenerator.GetInt32(uppercase.Length)]);
        password.Append(lowercase[RandomNumberGenerator.GetInt32(lowercase.Length)]);
        password.Append(digits[RandomNumberGenerator.GetInt32(digits.Length)]);
        password.Append(special[RandomNumberGenerator.GetInt32(special.Length)]);

        // Fill remaining characters randomly
        for (int i = 4; i < 12; i++)
        {
            password.Append(allChars[RandomNumberGenerator.GetInt32(allChars.Length)]);
        }

        // Shuffle the password
        return new string(password.ToString().OrderBy(_ => RandomNumberGenerator.GetInt32(int.MaxValue)).ToArray());
    }

    public string HashPassword(string password)
    {
        return _passwordHasher.HashPassword(null!, password);
    }

    public bool VerifyPassword(string hashedPassword, string providedPassword)
    {
        var result = _passwordHasher.VerifyHashedPassword(null!, hashedPassword, providedPassword);
        return result == PasswordVerificationResult.Success;
    }
}