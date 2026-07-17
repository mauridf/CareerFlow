namespace CareerFlow.Core.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body, CancellationToken ct = default);
    Task SendEmailVerificationAsync(string to, string userName, string token, CancellationToken ct = default);
    Task SendPasswordResetAsync(string to, string userName, string token, CancellationToken ct = default);
}
