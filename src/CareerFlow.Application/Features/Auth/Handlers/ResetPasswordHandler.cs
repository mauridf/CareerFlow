using MediatR;
using Microsoft.Extensions.Logging;
using CareerFlow.Application.Common.Interfaces;
using CareerFlow.Application.Features.Auth.Commands;
using CareerFlow.Core.Entities;
using CareerFlow.Core.Exceptions;
using CareerFlow.Core.Interfaces;

namespace CareerFlow.Application.Features.Auth.Handlers;

public class ResetPasswordHandler : IRequestHandler<ResetPasswordCommand>
{
    private readonly IRepository<User> _userRepository;
    private readonly ITokenService _tokenService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ResetPasswordHandler> _logger;

    public ResetPasswordHandler(
        IRepository<User> userRepository,
        ITokenService tokenService,
        IPasswordHasher passwordHasher,
        IUnitOfWork unitOfWork,
        ILogger<ResetPasswordHandler> logger)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(ResetPasswordCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("🔑 Tentativa de redefinição de senha");

        var validation = await _tokenService.ValidatePurposeTokenAsync(command.Token, "password_reset", cancellationToken);

        if (!validation.IsValid || validation.UserId == null)
            throw new UnauthorizedException(validation.Error ?? "Token inválido ou expirado");

        var user = await _userRepository.GetByIdAsync(validation.UserId.Value, cancellationToken)
            ?? throw new NotFoundException("Usuário");

        if (user.DeletedAt.HasValue)
            throw new UnauthorizedException("Conta não encontrada");

        var newPasswordHash = _passwordHasher.HashPassword(command.NewPassword);
        user.ChangePassword(newPasswordHash);

        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("✅ Senha redefinida com sucesso: {UserId}", user.Id);
    }
}
