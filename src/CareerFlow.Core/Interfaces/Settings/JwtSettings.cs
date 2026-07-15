namespace CareerFlow.Core.Interfaces.Settings;

/// <summary>
/// Configurações de autenticação JWT
/// </summary>
public class JwtSettings
{
    public const string SectionName = "Jwt";

    /// <summary>Chave secreta para assinatura (mínimo 512 bits)</summary>
    public string SecretKey { get; set; } = string.Empty;

    /// <summary>Emissor do token</summary>
    public string Issuer { get; set; } = "careerflow-api";

    /// <summary>Audiência do token</summary>
    public string Audience { get; set; } = "careerflow-app";

    /// <summary>Expiração do access token em minutos</summary>
    public int AccessTokenExpirationMinutes { get; set; } = 15;

    /// <summary>Expiração do refresh token em dias</summary>
    public int RefreshTokenExpirationDays { get; set; } = 7;
}
