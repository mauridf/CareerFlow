namespace CareerFlow.Application.Common.Interfaces;

/// <summary>
/// Serviço para hash e verificação de senhas.
/// </summary>
public interface IPasswordHasher
{
    /// <summary>Gera o hash de uma senha</summary>
    string HashPassword(string password);

    /// <summary>Verifica se a senha corresponde ao hash</summary>
    bool VerifyPassword(string password, string passwordHash);
}
