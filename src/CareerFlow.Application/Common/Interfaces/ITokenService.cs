using CareerFlow.Core.Entities;

namespace CareerFlow.Application.Common.Interfaces;

/// <summary>
/// Serviço para geração e validação de tokens JWT.
/// </summary>
public interface ITokenService
{
    /// <summary>Gera um par de tokens (access + refresh)</summary>
    Task<TokenResult> GenerateTokensAsync(User user, CancellationToken cancellationToken = default);

    /// <summary>Gera apenas o access token</summary>
    string GenerateAccessToken(User user);

    /// <summary>Gera apenas o refresh token</summary>
    string GenerateRefreshToken(User user);

    /// <summary>Valida e extrai claims de um access token</summary>
    Task<TokenValidationResult> ValidateAccessTokenAsync(string token, CancellationToken cancellationToken = default);

    /// <summary>Gera token para verificação de email</summary>
    string GenerateEmailVerificationToken(User user);

    /// <summary>Gera token para redefinição de senha</summary>
    string GeneratePasswordResetToken(User user);

    /// <summary>Valida um token de propósito específico e extrai o UserId</summary>
    Task<EmailTokenValidationResult> ValidatePurposeTokenAsync(string token, string purpose, CancellationToken cancellationToken = default);
}

public class TokenResult
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime AccessTokenExpiresAt { get; set; }
    public DateTime RefreshTokenExpiresAt { get; set; }
    public string TokenType { get; set; } = "Bearer";
}

public class EmailTokenValidationResult
{
    public bool IsValid { get; set; }
    public Guid? UserId { get; set; }
    public string? Email { get; set; }
    public string? Error { get; set; }
}

public class TokenValidationResult
{
    public bool IsValid { get; set; }
    public Guid? UserId { get; set; }
    public string? Email { get; set; }
    public string? Role { get; set; }
    public string? Error { get; set; }
}
