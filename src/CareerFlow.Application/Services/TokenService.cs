using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using CareerFlow.Application.Common.Interfaces;
using CareerFlow.Core.Entities;
using CareerFlow.Core.Interfaces.Settings;
using TokenValidationResult = CareerFlow.Application.Common.Interfaces.TokenValidationResult;
using EmailTokenValidationResult = CareerFlow.Application.Common.Interfaces.EmailTokenValidationResult;

namespace CareerFlow.Application.Services;

/// <summary>
/// Implementação do serviço de tokens JWT.
/// </summary>
public class TokenService : ITokenService
{
    private readonly JwtSettings _jwtSettings;
    private readonly byte[] _secretKeyBytes;

    public TokenService(IOptions<JwtSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings.Value;
        _secretKeyBytes = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);

        if (_secretKeyBytes.Length < 64) // 512 bits = 64 bytes
            throw new ArgumentException("JWT SecretKey deve ter no mínimo 512 bits (64 bytes)");
    }

    public Task<TokenResult> GenerateTokensAsync(User user, CancellationToken cancellationToken = default)
    {
        var accessToken = GenerateAccessToken(user);
        var refreshToken = GenerateRefreshToken(user);

        var result = new TokenResult
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
            RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays),
            TokenType = "Bearer"
        };

        return Task.FromResult(result);
    }

    public string GenerateAccessToken(User user)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.GivenName, user.Name),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
            new(ClaimTypes.Role, user.Role.ToString()),
            new("isPremium", user.IsPremium.ToString().ToLower()),
            new("emailVerified", user.IsEmailVerified().ToString().ToLower())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(_secretKeyBytes),
                SecurityAlgorithms.HmacSha512)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public string GenerateRefreshToken(User user)
    {
        var randomBytes = new byte[64]; // 512 bits
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    public Task<TokenValidationResult> ValidateAccessTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        try
        {
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(_secretKeyBytes),
                ValidateIssuer = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtSettings.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

            if (validatedToken is not JwtSecurityToken jwtToken)
            {
                return Task.FromResult(new TokenValidationResult
                {
                    IsValid = false,
                    Error = "Token inválido"
                });
            }

            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var email = principal.FindFirst(ClaimTypes.Email)?.Value;
            var role = principal.FindFirst(ClaimTypes.Role)?.Value;

            return Task.FromResult(new TokenValidationResult
            {
                IsValid = true,
                UserId = userId != null ? Guid.Parse(userId) : null,
                Email = email,
                Role = role
            });
        }
        catch (SecurityTokenExpiredException)
        {
            return Task.FromResult(new TokenValidationResult
            {
                IsValid = false,
                Error = "Token expirado"
            });
        }
        catch (Exception ex)
        {
            return Task.FromResult(new TokenValidationResult
            {
                IsValid = false,
                Error = ex.Message
            });
        }
    }

    public Task<TokenValidationResult> ExtractClaimsFromTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        try
        {
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(_secretKeyBytes),
                ValidateIssuer = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtSettings.Audience,
                ValidateLifetime = false,
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

            if (validatedToken is not JwtSecurityToken jwtToken)
            {
                return Task.FromResult(new TokenValidationResult
                {
                    IsValid = false,
                    Error = "Token inválido"
                });
            }

            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var email = principal.FindFirst(ClaimTypes.Email)?.Value;
            var role = principal.FindFirst(ClaimTypes.Role)?.Value;

            return Task.FromResult(new TokenValidationResult
            {
                IsValid = true,
                UserId = userId != null ? Guid.Parse(userId) : null,
                Email = email,
                Role = role
            });
        }
        catch (Exception ex)
        {
            return Task.FromResult(new TokenValidationResult
            {
                IsValid = false,
                Error = ex.Message
            });
        }
    }

    public string GenerateEmailVerificationToken(User user)
    {
        return GeneratePurposeToken(user, "email_verification", 48);
    }

    public string GeneratePasswordResetToken(User user)
    {
        return GeneratePurposeToken(user, "password_reset", 1);
    }

    private string GeneratePurposeToken(User user, string purpose, int expirationHours)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new("purpose", purpose)
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(expirationHours),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(_secretKeyBytes),
                SecurityAlgorithms.HmacSha512)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public Task<EmailTokenValidationResult> ValidatePurposeTokenAsync(string token, string purpose, CancellationToken cancellationToken = default)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        try
        {
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(_secretKeyBytes),
                ValidateIssuer = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtSettings.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

            if (validatedToken is not JwtSecurityToken jwtToken)
            {
                return Task.FromResult(new EmailTokenValidationResult
                {
                    IsValid = false,
                    Error = "Token inválido"
                });
            }

            var tokenPurpose = principal.FindFirst("purpose")?.Value;
            if (tokenPurpose != purpose)
            {
                return Task.FromResult(new EmailTokenValidationResult
                {
                    IsValid = false,
                    Error = "Token inválido para esta operação"
                });
            }

            var userId = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            var email = principal.FindFirst(JwtRegisteredClaimNames.Email)?.Value;

            return Task.FromResult(new EmailTokenValidationResult
            {
                IsValid = true,
                UserId = userId != null ? Guid.Parse(userId) : null,
                Email = email
            });
        }
        catch (SecurityTokenExpiredException)
        {
            return Task.FromResult(new EmailTokenValidationResult
            {
                IsValid = false,
                Error = "Token expirado"
            });
        }
        catch (Exception ex)
        {
            return Task.FromResult(new EmailTokenValidationResult
            {
                IsValid = false,
                Error = ex.Message
            });
        }
    }
}
