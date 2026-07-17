using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using CareerFlow.Core.Interfaces;
using CareerFlow.Core.Interfaces.Settings;

namespace CareerFlow.Infrastructure.External.Email;

public class SmtpEmailService : IEmailService
{
    private readonly EmailSettings _settings;
    private readonly ILogger<SmtpEmailService> _logger;

    public SmtpEmailService(IOptions<EmailSettings> settings, ILogger<SmtpEmailService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task SendEmailAsync(string to, string subject, string body, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(_settings.SmtpUsername) || string.IsNullOrWhiteSpace(_settings.SmtpPassword))
        {
            _logger.LogWarning("📧 Email não enviado para {To}: SMTP não configurado (SmtpUsername/SmtpPassword vazios)", to);
            _logger.LogInformation("📧 [MOCK] Para: {To} | Assunto: {Subject}", to, subject);
            return;
        }

        try
        {
            using var client = new SmtpClient(_settings.SmtpHost, _settings.SmtpPort)
            {
                Credentials = new NetworkCredential(_settings.SmtpUsername, _settings.SmtpPassword),
                EnableSsl = true
            };

            using var message = new MailMessage
            {
                From = new MailAddress(_settings.SmtpUsername, _settings.FromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            message.To.Add(to);

            await client.SendMailAsync(message, ct);

            _logger.LogInformation("✅ Email enviado para {To}: {Subject}", to, subject);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Falha ao enviar email para {To}: {Subject}", to, subject);
            throw;
        }
    }

    public async Task SendEmailVerificationAsync(string to, string userName, string token, CancellationToken ct = default)
    {
        var link = $"{_settings.FrontendUrl}/verify-email?token={token}";
        var subject = "Verifique seu email - CareerFlow";

        var body = $@"
<!DOCTYPE html>
<html>
<head><meta charset='utf-8'></head>
<body style='font-family: Arial, sans-serif; padding: 20px;'>
    <h2>Olá, {userName}!</h2>
    <p>Obrigado por se cadastrar no CareerFlow.</p>
    <p>Clique no link abaixo para verificar seu email:</p>
    <p><a href='{link}' style='display: inline-block; padding: 12px 24px; background: #6366f1; color: white; text-decoration: none; border-radius: 6px;'>Verificar Email</a></p>
    <p>Ou copie e cole este link no navegador:</p>
    <p style='color: #6366f1;'>{link}</p>
    <p>Este link expira em 48 horas.</p>
    <p>Se você não fez este cadastro, ignore este email.</p>
    <hr>
    <p style='color: #888; font-size: 12px;'>CareerFlow - Gerencie seu currículo profissional</p>
</body>
</html>";

        await SendEmailAsync(to, subject, body, ct);
    }

    public async Task SendPasswordResetAsync(string to, string userName, string token, CancellationToken ct = default)
    {
        var link = $"{_settings.FrontendUrl}/reset-password?token={token}";
        var subject = "Redefinição de senha - CareerFlow";

        var body = $@"
<!DOCTYPE html>
<html>
<head><meta charset='utf-8'></head>
<body style='font-family: Arial, sans-serif; padding: 20px;'>
    <h2>Olá, {userName}!</h2>
    <p>Recebemos uma solicitação de redefinição de senha para sua conta no CareerFlow.</p>
    <p>Clique no link abaixo para criar uma nova senha:</p>
    <p><a href='{link}' style='display: inline-block; padding: 12px 24px; background: #6366f1; color: white; text-decoration: none; border-radius: 6px;'>Redefinir Senha</a></p>
    <p>Ou copie e cole este link no navegador:</p>
    <p style='color: #6366f1;'>{link}</p>
    <p>Este link expira em 1 hora.</p>
    <p>Se você não solicitou esta redefinição, ignore este email.</p>
    <hr>
    <p style='color: #888; font-size: 12px;'>CareerFlow - Gerencie seu currículo profissional</p>
</body>
</html>";

        await SendEmailAsync(to, subject, body, ct);
    }
}
