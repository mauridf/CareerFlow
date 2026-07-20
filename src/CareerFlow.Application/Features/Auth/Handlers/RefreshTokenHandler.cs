using MediatR;
using Microsoft.Extensions.Logging;
using CareerFlow.Application.Common.Interfaces;
using CareerFlow.Application.Features.Auth.Commands;
using CareerFlow.Application.Features.Auth.DTOs;
using CareerFlow.Core.Entities;
using CareerFlow.Core.Exceptions;
using CareerFlow.Core.Interfaces;

namespace CareerFlow.Application.Features.Auth.Handlers;

public class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, AuthResponse>
{
    private readonly IRepository<User> _userRepository;
    private readonly ITokenService _tokenService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RefreshTokenHandler> _logger;

    public RefreshTokenHandler(
        IRepository<User> userRepository,
        ITokenService tokenService,
        IUnitOfWork unitOfWork,
        ILogger<RefreshTokenHandler> logger)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<AuthResponse> Handle(RefreshTokenCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("🔄 Tentativa de refresh token");

        var validation = await _tokenService.ExtractClaimsFromTokenAsync(command.AccessToken, cancellationToken);

        if (!validation.IsValid || validation.UserId == null)
            throw new UnauthorizedException("Token de acesso inválido ou expirado");

        var user = await _userRepository.GetByIdAsync(validation.UserId.Value, cancellationToken)
            ?? throw new UnauthorizedException("Usuário não encontrado");

        if (!user.CanLogin())
            throw new UnauthorizedException("Conta inativa ou bloqueada");

        var tokens = await _tokenService.GenerateTokensAsync(user, cancellationToken);

        _logger.LogInformation("✅ Token renovado: {UserId}", user.Id);

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
