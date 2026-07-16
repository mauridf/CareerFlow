namespace CareerFlow.Application.Features.Auth.DTOs;

// ============================================
// Request DTOs
// ============================================

public record RegisterUserRequest(
    string Name,
    string Email,
    string Password);

public record LoginRequest(
    string Email,
    string Password);

public record RefreshTokenRequest(
    string AccessToken,
    string RefreshToken);

public record ForgotPasswordRequest(
    string Email);

public record ResetPasswordRequest(
    string Email,
    string Token,
    string NewPassword);

public record ChangePasswordRequest(
    string CurrentPassword,
    string NewPassword);

// ============================================
// Response DTOs
// ============================================

public class AuthResponse
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsPremium { get; set; }
    public bool EmailVerified { get; set; }
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime AccessTokenExpiresAt { get; set; }
    public string TokenType { get; set; } = "Bearer";
}

public class UserProfileResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsPremium { get; set; }
    public DateTime? PremiumUntil { get; set; }
    public bool EmailVerified { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
