using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CareerFlow.Application.Features.Auth.Commands;
using CareerFlow.Application.Features.Auth.DTOs;
using CareerFlow.Core.Interfaces;

namespace CareerFlow.Api.Controllers;

/// <summary>
/// Controller responsável pela autenticação de usuários.
/// </summary>
[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IMediator mediator,
        ICurrentUserService currentUser,
        ILogger<AuthController> logger)
    {
        _mediator = mediator;
        _currentUser = currentUser;
        _logger = logger;
    }

    /// <summary>
    /// Registra um novo usuário
    /// </summary>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
    {
        _logger.LogInformation("📝 Requisição de registro: {Email}", request.Email);

        var command = new RegisterUserCommand(request.Name, request.Email, request.Password);
        var result = await _mediator.Send(command);

        _logger.LogInformation("✅ Usuário registrado: {UserId}", result.UserId);

        return Created($"/api/v1/users/{result.UserId}", new
        {
            success = true,
            data = result,
            meta = new { timestamp = DateTime.UtcNow }
        });
    }

    /// <summary>
    /// Realiza login do usuário
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        _logger.LogInformation("🔑 Tentativa de login: {Email}", request.Email);

        var command = new LoginCommand(request.Email, request.Password);
        var result = await _mediator.Send(command);

        _logger.LogInformation("✅ Login bem-sucedido: {UserId}", result.UserId);

        return Ok(new
        {
            success = true,
            data = result,
            meta = new { timestamp = DateTime.UtcNow }
        });
    }

    /// <summary>
    /// Renova o access token usando refresh token
    /// </summary>
    [HttpPost("refresh")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var command = new RefreshTokenCommand(request.AccessToken, request.RefreshToken);
        var result = await _mediator.Send(command);

        return Ok(new
        {
            success = true,
            data = result,
            meta = new { timestamp = DateTime.UtcNow }
        });
    }

    /// <summary>
    /// Obtém o perfil do usuário autenticado
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(UserProfileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult GetCurrentUser()
    {
        if (!_currentUser.IsAuthenticated)
            return Unauthorized();

        var profile = new UserProfileResponse
        {
            Id = _currentUser.UserId!.Value,
            Name = _currentUser.Name ?? "Usuário",
            Email = _currentUser.Email ?? "",
            Role = _currentUser.Role ?? "User",
            IsPremium = _currentUser.IsPremium
        };

        return Ok(new
        {
            success = true,
            data = profile,
            meta = new { timestamp = DateTime.UtcNow }
        });
    }

    /// <summary>
    /// Solicita recuperação de senha
    /// </summary>
    [HttpPost("forgot-password")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        _logger.LogInformation("📧 Solicitação de recuperação de senha: {Email}", request.Email);

        await _mediator.Send(new ForgotPasswordCommand(request.Email));

        return Ok(new
        {
            success = true,
            data = new { message = "Se o email existir, um link de recuperação será enviado." },
            meta = new { timestamp = DateTime.UtcNow }
        });
    }

    /// <summary>
    /// Redefine a senha
    /// </summary>
    [HttpPost("reset-password")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        await _mediator.Send(new ResetPasswordCommand(request.Token, request.NewPassword));

        return Ok(new
        {
            success = true,
            data = new { message = "Senha redefinida com sucesso." },
            meta = new { timestamp = DateTime.UtcNow }
        });
    }

    /// <summary>
    /// Altera a senha do usuário autenticado
    /// </summary>
    [HttpPost("change-password")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        await _mediator.Send(new ChangePasswordCommand(request.CurrentPassword, request.NewPassword));

        return Ok(new
        {
            success = true,
            data = new { message = "Senha alterada com sucesso." },
            meta = new { timestamp = DateTime.UtcNow }
        });
    }

    /// <summary>
    /// Verifica o email do usuário
    /// </summary>
    [HttpPost("verify-email")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailRequest request)
    {
        _logger.LogInformation("📧 Requisição de verificação de email");

        await _mediator.Send(new VerifyEmailCommand(request.Token));

        return Ok(new
        {
            success = true,
            data = new { message = "Email verificado com sucesso." },
            meta = new { timestamp = DateTime.UtcNow }
        });
    }

    /// <summary>
    /// Reenvia o email de verificação
    /// </summary>
    [HttpPost("resend-verification")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ResendVerification()
    {
        _logger.LogInformation("📧 Reenvio de verificação de email");

        await _mediator.Send(new ResendVerificationCommand());

        return Ok(new
        {
            success = true,
            data = new { message = "Email de verificação reenviado." },
            meta = new { timestamp = DateTime.UtcNow }
        });
    }
}
