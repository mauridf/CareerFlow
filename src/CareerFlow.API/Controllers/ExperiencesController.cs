using CareerFlow.Application.DTOs;
using CareerFlow.Application.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareerFlow.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ExperiencesController : ApiControllerBase
{
    private readonly IExperienceService _experienceService;
    private readonly IValidator<CreateProfessionalExperienceDto> _createExperienceValidator;

    public ExperiencesController(
        IExperienceService experienceService,
        IValidator<CreateProfessionalExperienceDto> createExperienceValidator)
    {
        _experienceService = experienceService;
        _createExperienceValidator = createExperienceValidator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProfessionalExperienceDto>>> GetExperiences()
    {
        var userId = GetUserId();
        var experiences = await _experienceService.GetUserExperiencesAsync(userId);
        return Ok(experiences);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProfessionalExperienceDto>> GetExperience(Guid id)
    {
        var experience = await _experienceService.GetExperienceByIdAsync(id);

        if (experience.UserId != GetUserId())
            return Forbid();

        return Ok(experience);
    }

    [HttpPost]
    public async Task<ActionResult<ProfessionalExperienceDto>> CreateExperience(CreateProfessionalExperienceDto dto)
    {
        var validationResult = await _createExperienceValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var userId = GetUserId();
        var experience = await _experienceService.CreateExperienceAsync(userId, dto);
        return CreatedAtAction(nameof(GetExperience), new { id = experience.Id }, experience);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ProfessionalExperienceDto>> UpdateExperience(Guid id, UpdateProfessionalExperienceDto dto)
    {
        var experience = await _experienceService.GetExperienceByIdAsync(id);

        if (experience.UserId != GetUserId())
            return Forbid();

        var updatedExperience = await _experienceService.UpdateExperienceAsync(id, dto);
        return Ok(updatedExperience);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteExperience(Guid id)
    {
        var experience = await _experienceService.GetExperienceByIdAsync(id);

        if (experience.UserId != GetUserId())
            return Forbid();

        var deleted = await _experienceService.DeleteExperienceAsync(id);

        return deleted ? NoContent() : NotFound();
    }

    [HttpPost("{experienceId}/skills/{skillId}")]
    public async Task<ActionResult> AddSkillToExperience(Guid experienceId, Guid skillId)
    {
        var experience = await _experienceService.GetExperienceByIdAsync(experienceId);

        if (experience.UserId != GetUserId())
            return Forbid();

        var success = await _experienceService.AddSkillToExperienceAsync(experienceId, skillId);

        return success ? NoContent() : BadRequest("Não foi possível adicionar a skill à experiência.");
    }

    [HttpDelete("{experienceId}/skills/{skillId}")]
    public async Task<ActionResult> RemoveSkillFromExperience(Guid experienceId, Guid skillId)
    {
        var experience = await _experienceService.GetExperienceByIdAsync(experienceId);

        if (experience.UserId != GetUserId())
            return Forbid();

        var success = await _experienceService.RemoveSkillFromExperienceAsync(experienceId, skillId);

        return success ? NoContent() : BadRequest("Não foi possível remover a skill da experiência.");
    }
}