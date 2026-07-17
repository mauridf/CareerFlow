using MediatR;
using Microsoft.Extensions.Logging;
using CareerFlow.Application.Common.Interfaces;
using CareerFlow.Application.Features.Auth.Commands;
using CareerFlow.Core.Entities;
using CareerFlow.Core.Exceptions;
using CareerFlow.Core.Interfaces;

namespace CareerFlow.Application.Features.Auth.Handlers;

public class ChangePasswordHandler : IRequestHandler<ChangePasswordCommand>
{
    private readonly ICurrentUserService _currentUser;
    private readonly IRepository<User> _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ChangePasswordHandler> _logger;

    public ChangePasswordHandler(
        ICurrentUserService currentUser,
        IRepository<User> userRepository,
        IPasswordHasher passwordHasher,
        IUnitOfWork unitOfWork,
        ILogger<ChangePasswordHandler> logger)
    {
        _currentUser = currentUser;
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(ChangePasswordCommand command, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId
            ?? throw new UnauthorizedException("Usuário não autenticado");

        _logger.LogInformation("🔑 Alteração de senha: {UserId}", userId);

        var user = await _userRepository.GetByIdAsync(userId, cancellationToken)
            ?? throw new NotFoundException("Usuário");

        if (!_passwordHasher.VerifyPassword(command.CurrentPassword, user.PasswordHash))
            throw new UnauthorizedException("Senha atual incorreta");

        var newPasswordHash = _passwordHasher.HashPassword(command.NewPassword);
        user.ChangePassword(newPasswordHash);

        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("✅ Senha alterada com sucesso: {UserId}", userId);
    }
}
