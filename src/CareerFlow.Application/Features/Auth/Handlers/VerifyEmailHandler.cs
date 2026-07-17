using MediatR;
using Microsoft.Extensions.Logging;
using CareerFlow.Application.Common.Interfaces;
using CareerFlow.Application.Features.Auth.Commands;
using CareerFlow.Core.Entities;
using CareerFlow.Core.Exceptions;
using CareerFlow.Core.Interfaces;

namespace CareerFlow.Application.Features.Auth.Handlers;

public class VerifyEmailHandler : IRequestHandler<VerifyEmailCommand>
{
    private readonly IRepository<User> _userRepository;
    private readonly ITokenService _tokenService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<VerifyEmailHandler> _logger;

    public VerifyEmailHandler(
        IRepository<User> userRepository,
        ITokenService tokenService,
        IUnitOfWork unitOfWork,
        ILogger<VerifyEmailHandler> logger)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(VerifyEmailCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("📧 Tentativa de verificação de email");

        var validation = await _tokenService.ValidatePurposeTokenAsync(command.Token, "email_verification", cancellationToken);

        if (!validation.IsValid || validation.UserId == null)
            throw new UnauthorizedException(validation.Error ?? "Token inválido ou expirado");

        var user = await _userRepository.GetByIdAsync(validation.UserId.Value, cancellationToken)
            ?? throw new NotFoundException("Usuário");

        if (user.IsEmailVerified())
        {
            _logger.LogWarning("⚠️ Email já verificado: {UserId}", user.Id);
            return;
        }

        user.VerifyEmail();

        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("✅ Email verificado com sucesso: {UserId} - {Email}", user.Id, user.Email);
    }
}
