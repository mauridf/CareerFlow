using CareerFlow.Application.Common.Interfaces;

namespace CareerFlow.Application.Services;

/// <summary>
/// Implementação do hash de senhas usando BCrypt.
/// </summary>
public class PasswordService : IPasswordHasher
{
    private const int WorkFactor = 12; // Custo do BCrypt (12 = ~250ms)

    public string HashPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Senha não pode ser vazia", nameof(password));

        if (password.Length < 8)
            throw new ArgumentException("Senha deve ter no mínimo 8 caracteres", nameof(password));

        return BCrypt.Net.BCrypt.HashPassword(password, WorkFactor);
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(passwordHash))
            return false;

        try
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }
        catch
        {
            return false;
        }
    }
}
