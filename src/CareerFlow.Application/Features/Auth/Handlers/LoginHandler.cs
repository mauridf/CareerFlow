using MediatR;
using Microsoft.Extensions.Logging;
using CareerFlow.Application.Common.Interfaces;
using CareerFlow.Application.Features.Auth.Commands;
using CareerFlow.Application.Features.Auth.DTOs;
using CareerFlow.Core.Exceptions;
using CareerFlow.Core.Entities;
using CareerFlow.Core.Interfaces;

namespace CareerFlow.Application.Features.Auth.Handlers;

public class LoginHandler : IRequestHandler<LoginCommand, AuthResponse>
{
    private readonly IRepository<User> _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<LoginHandler> _logger;

    public LoginHandler(
        IRepository<User> userRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        IUnitOfWork unitOfWork,
        ILogger<LoginHandler> logger)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<AuthResponse> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("🔑 Tentativa de login: {Email}", command.Email);

        // Busca usuário
        var users = await _userRepository.FindAsync(
            u => u.Email == command.Email.ToLowerInvariant().Trim() && u.DeletedAt == null,
            cancellationToken);
        var user = users.FirstOrDefault();

        if (user == null)
        {
            _logger.LogWarning("❌ Login falhou: usuário não encontrado - {Email}", command.Email);
            throw new UnauthorizedException("Email ou senha inválidos");
        }

        // Verifica se pode fazer login
        if (!user.CanLogin())
        {
            if (user.IsLocked())
            {
                _logger.LogWarning("🔒 Conta bloqueada: {Email} até {Until}", user.Email, user.LockedUntil);
                throw new UnauthorizedException($"Conta bloqueada. Tente novamente após {user.LockedUntil:HH:mm}");
            }

            _logger.LogWarning("❌ Conta inativa: {Email}", user.Email);
            throw new UnauthorizedException("Conta inativa. Verifique seu email para ativar a conta.");
        }

        // Verifica senha
        if (!_passwordHasher.VerifyPassword(command.Password, user.PasswordHash))
        {
            user.RecordFailedLogin();
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogWarning("❌ Senha incorreta: {Email} (tentativa {Attempt})",
                user.Email, user.FailedLoginAttempts);

            throw new UnauthorizedException("Email ou senha inválidos");
        }

        // Login bem-sucedido
        user.RecordSuccessfulLogin();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Gera tokens
        var tokens = await _tokenService.GenerateTokensAsync(user, cancellationToken);

        _logger.LogInformation("✅ Login bem-sucedido: {UserId} - {Email}", user.Id, user.Email);

        return new AuthResponse
        {
            UserId = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role.ToString(),
            IsPremium = user.IsPremium,
            EmailVerified = user.IsEmailVerified(),
            AccessToken = tokens.AccessToken,
            RefreshToken = tokens.RefreshToken,
            AccessTokenExpiresAt = tokens.AccessTokenExpiresAt,
            TokenType = tokens.TokenType
        };
    }
}
