using MediatR;
using Microsoft.Extensions.Logging;
using CareerFlow.Application.Common.Interfaces;
using CareerFlow.Application.Features.Auth.Commands;
using CareerFlow.Core.Entities;
using CareerFlow.Core.Exceptions;
using CareerFlow.Core.Interfaces;

namespace CareerFlow.Application.Features.Auth.Handlers;

public class ResendVerificationHandler : IRequestHandler<ResendVerificationCommand>
{
    private readonly ICurrentUserService _currentUser;
    private readonly IRepository<User> _userRepository;
    private readonly ITokenService _tokenService;
    private readonly IEmailService _emailService;
    private readonly ILogger<ResendVerificationHandler> _logger;

    public ResendVerificationHandler(
        ICurrentUserService currentUser,
        IRepository<User> userRepository,
        ITokenService tokenService,
        IEmailService emailService,
        ILogger<ResendVerificationHandler> logger)
    {
        _currentUser = currentUser;
        _userRepository = userRepository;
        _tokenService = tokenService;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task Handle(ResendVerificationCommand command, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId
            ?? throw new UnauthorizedException("Usuário não autenticado");

        _logger.LogInformation("📧 Reenvio de verificação de email: {UserId}", userId);

        var user = await _userRepository.GetByIdAsync(userId, cancellationToken)
            ?? throw new NotFoundException("Usuário");

        if (user.IsEmailVerified())
        {
            _logger.LogWarning("⚠️ Email já verificado: {UserId}", user.Id);
            throw new DomainException("Email já foi verificado");
        }

        var token = _tokenService.GenerateEmailVerificationToken(user);

        await _emailService.SendEmailVerificationAsync(user.Email, user.Name, token, cancellationToken);

        _logger.LogInformation("✅ Email de verificação reenviado para: {Email}", user.Email);
    }
}
