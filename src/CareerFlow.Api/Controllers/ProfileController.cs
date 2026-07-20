using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CareerFlow.Application.Common.Interfaces;
using CareerFlow.Application.Features.Profile.Commands;
using CareerFlow.Application.Features.Profile.DTOs;
using CareerFlow.Application.Features.Profile.Queries;
using CareerFlow.Core.Exceptions;
using CareerFlow.Core.Interfaces;

namespace CareerFlow.Api.Controllers;

/// <summary>
/// Controller para gerenciamento do perfil pessoal do usuário.
/// </summary>
[ApiController]
[Route("api/v1/profile")]
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ProfileController> _logger;
    private readonly IPersonRepository _personRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public ProfileController(
        IMediator mediator,
        ILogger<ProfileController> logger,
        IPersonRepository personRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser)
    {
        _mediator = mediator;
        _logger = logger;
        _personRepository = personRepository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    /// <summary>
    /// Obtém o perfil do usuário autenticado
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ProfileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProfile()
    {
        var result = await _mediator.Send(new GetProfileQuery());

        return Ok(new { success = true, data = result, meta = new { timestamp = DateTime.UtcNow } });
    }

    /// <summary>
    /// Cria o perfil do usuário (após registro)
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ProfileResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateProfile([FromBody] CreateProfileRequest request)
    {
        var command = new CreateProfileCommand(
            request.Phone,
            request.City,
            request.State,
            request.BirthDate,
            request.ProfessionalSummary,
            request.CurrentPosition,
            request.CurrentCompany);

        var result = await _mediator.Send(command);

        return Created($"/api/v1/profile", new { success = true, data = result, meta = new { timestamp = DateTime.UtcNow } });
    }

    /// <summary>
    /// Atualiza o perfil do usuário
    /// </summary>
    [HttpPut]
    [ProducesResponseType(typeof(ProfileResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
    {
        var command = new UpdateProfileCommand(
            request.Phone,
            request.City,
            request.State,
            request.BirthDate,
            request.ProfessionalSummary,
            request.CurrentPosition,
            request.CurrentCompany);

        var result = await _mediator.Send(command);

        return Ok(new { success = true, data = result, meta = new { timestamp = DateTime.UtcNow } });
    }

    /// <summary>
    /// Obtém o percentual de completude do perfil
    /// </summary>
    [HttpGet("completion")]
    [ProducesResponseType(typeof(ProfileCompletionResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProfileCompletion()
    {
        var result = await _mediator.Send(new GetProfileCompletionQuery());

        return Ok(new { success = true, data = result, meta = new { timestamp = DateTime.UtcNow } });
    }

    /// <summary>
    /// Upload de foto de perfil
    /// </summary>
    [HttpPost("photo")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UploadPhoto(IFormFile photo)
    {
        if (photo == null || photo.Length == 0)
            return BadRequest(new { success = false, error = new { code = "VALIDATION_ERROR", message = "Foto é obrigatória" } });

        // Valida tamanho (máx 5MB)
        if (photo.Length > 5 * 1024 * 1024)
            return BadRequest(new { success = false, error = new { code = "VALIDATION_ERROR", message = "Foto deve ter no máximo 5MB" } });

        // Valida formato
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        var extension = Path.GetExtension(photo.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(extension))
            return BadRequest(new { success = false, error = new { code = "VALIDATION_ERROR", message = "Formato inválido. Use JPG, PNG ou WebP" } });

        // Salva arquivo localmente
        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads", "photos");
        Directory.CreateDirectory(uploadsFolder);

        var fileName = $"{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(uploadsFolder, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await photo.CopyToAsync(stream);
        }

        var photoUrl = $"/uploads/photos/{fileName}";

        // Atualiza a URL da foto no perfil do usuário
        var userId = _currentUser.UserId;
        if (userId.HasValue)
        {
            var person = await _personRepository.GetByUserIdAsync(userId.Value, cancellationToken: default);
            if (person != null)
            {
                person.UpdatePhoto(photoUrl);
                _personRepository.Update(person);
                await _unitOfWork.SaveChangesAsync(default);
            }
        }

        _logger.LogInformation("📸 Foto de perfil enviada: {FileName}", fileName);

        return Ok(new
        {
            success = true,
            data = new { photoUrl },
            meta = new { timestamp = DateTime.UtcNow }
        });
    }

    /// <summary>
    /// Remove a foto de perfil
    /// </summary>
    [HttpDelete("photo")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> DeletePhoto()
    {
        _logger.LogInformation("🗑️ Foto de perfil removida");

        // Remove a URL da foto no perfil do usuário
        var userId = _currentUser.UserId;
        if (userId.HasValue)
        {
            var person = await _personRepository.GetByUserIdAsync(userId.Value, cancellationToken: default);
            if (person != null)
            {
                person.RemovePhoto();
                _personRepository.Update(person);
                await _unitOfWork.SaveChangesAsync(default);
            }
        }

        return Ok(new
        {
            success = true,
            data = new { message = "Foto removida com sucesso" },
            meta = new { timestamp = DateTime.UtcNow }
        });
    }
}
