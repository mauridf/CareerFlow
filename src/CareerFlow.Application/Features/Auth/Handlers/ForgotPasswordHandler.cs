using MediatR;
using Microsoft.Extensions.Logging;
using CareerFlow.Application.Common.Interfaces;
using CareerFlow.Application.Features.Auth.Commands;
using CareerFlow.Core.Entities;
using CareerFlow.Core.Interfaces;

namespace CareerFlow.Application.Features.Auth.Handlers;

public class ForgotPasswordHandler : IRequestHandler<ForgotPasswordCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly IEmailService _emailService;
    private readonly ILogger<ForgotPasswordHandler> _logger;

    public ForgotPasswordHandler(
        IUserRepository userRepository,
        ITokenService tokenService,
        IEmailService emailService,
        ILogger<ForgotPasswordHandler> logger)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task Handle(ForgotPasswordCommand command, CancellationToken cancellationToken)
    {
        var email = command.Email.ToLowerInvariant().Trim();
        _logger.LogInformation("📧 Solicitação de redefinição de senha: {Email}", email);

        var user = await _userRepository.GetByEmailAsync(email, cancellationToken);

        if (user == null || user.DeletedAt.HasValue)
        {
            _logger.LogWarning("❌ Email não encontrado: {Email}", email);
            return;
        }

        var token = _tokenService.GeneratePasswordResetToken(user);

        await _emailService.SendPasswordResetAsync(user.Email, user.Name, token, cancellationToken);

        _logger.LogInformation("✅ Email de redefinição enviado para: {Email}", email);
    }
}
