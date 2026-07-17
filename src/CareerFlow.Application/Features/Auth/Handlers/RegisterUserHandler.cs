using MediatR;
using Microsoft.Extensions.Logging;
using CareerFlow.Application.Common.Interfaces;
using CareerFlow.Application.Features.Auth.Commands;
using CareerFlow.Application.Features.Auth.DTOs;
using CareerFlow.Core.Entities;
using CareerFlow.Core.Exceptions;
using CareerFlow.Core.Interfaces;

namespace CareerFlow.Application.Features.Auth.Handlers;

public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, AuthResponse>
{
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<Person> _personRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly IEmailService _emailService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RegisterUserHandler> _logger;

    public RegisterUserHandler(
        IRepository<User> userRepository,
        IRepository<Person> personRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        IEmailService emailService,
        IUnitOfWork unitOfWork,
        ILogger<RegisterUserHandler> logger)
    {
        _userRepository = userRepository;
        _personRepository = personRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _emailService = emailService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<AuthResponse> Handle(RegisterUserCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("📝 Registrando novo usuário: {Email}", command.Email);

        // Verifica se email já existe
        if (await _userRepository.ExistsAsync(u => u.Email == command.Email.ToLowerInvariant().Trim(), cancellationToken))
        {
            throw new ConflictException("Usuário", "email", command.Email);
        }

        // Cria usuário
        var passwordHash = _passwordHasher.HashPassword(command.Password);
        var user = User.Create(command.Name, command.Email, passwordHash);

        // Cria perfil automaticamente
        var person = Person.Create(user.Id);
        person.SetResumeSlug(person.GenerateSlug(command.Name));

        await _userRepository.AddAsync(user, cancellationToken);
        await _personRepository.AddAsync(person, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Envia email de verificação
        try
        {
            var verificationToken = _tokenService.GenerateEmailVerificationToken(user);
            await _emailService.SendEmailVerificationAsync(user.Email, user.Name, verificationToken, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "⚠️ Falha ao enviar email de verificação para {Email}", user.Email);
        }

        // Gera tokens
        var tokens = await _tokenService.GenerateTokensAsync(user, cancellationToken);

        _logger.LogInformation("✅ Usuário registrado: {UserId} - {Email}", user.Id, user.Email);

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
