namespace HRSystem.Application.Common.Interfaces;

public interface IPasswordService
{
    string GenerateRandomPassword();
    string HashPassword(string password);
    bool VerifyPassword(string hashedPassword, string providedPassword);
}