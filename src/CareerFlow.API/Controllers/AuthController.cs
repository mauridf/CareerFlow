using CareerFlow.Application.DTOs;
using CareerFlow.Application.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace CareerFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ApiControllerBase
{
    private readonly IUserService _userService;
    private readonly IValidator<CreateUserDto> _createUserValidator;
    private readonly IValidator<LoginDto> _loginValidator;

    public AuthController(
        IUserService userService,
        IValidator<CreateUserDto> createUserValidator,
        IValidator<LoginDto> loginValidator)
    {
        _userService = userService;
        _createUserValidator = createUserValidator;
        _loginValidator = loginValidator;
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(CreateUserDto createUserDto)
    {
        var validationResult = await _createUserValidator.ValidateAsync(createUserDto);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        try
        {
            var user = await _userService.CreateAsync(createUserDto);
            return CreatedAtAction(nameof(Register), new { id = user.Id }, user);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto loginDto)
    {
        var validationResult = await _loginValidator.ValidateAsync(loginDto);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        try
        {
            var authResponse = await _userService.AuthenticateAsync(loginDto);
            return Ok(authResponse);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new { message = "Email ou senha inválidos." });
        }
    }

    [HttpGet("check-email")]
    public async Task<ActionResult<bool>> CheckEmailExists([FromQuery] string email)
    {
        if (string.IsNullOrEmpty(email))
            return BadRequest(new { message = "Email é obrigatório." });

        var exists = await _userService.EmailExistsAsync(email);
        return Ok(exists);
    }
}