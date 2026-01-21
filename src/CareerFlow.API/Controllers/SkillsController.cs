using CareerFlow.Application.DTOs;
using CareerFlow.Application.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareerFlow.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class SkillsController : ApiControllerBase
{
    private readonly ISkillService _skillService;
    private readonly IValidator<CreateSkillDto> _createSkillValidator;

    public SkillsController(
        ISkillService skillService,
        IValidator<CreateSkillDto> createSkillValidator)
    {
        _skillService = skillService;
        _createSkillValidator = createSkillValidator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SkillDto>>> GetSkills([FromQuery] SkillFilterDto? filter)
    {
        var userId = GetUserId();
        var skills = await _skillService.GetUserSkillsAsync(userId, filter);
        return Ok(skills);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SkillDto>> GetSkill(Guid id)
    {
        var skill = await _skillService.GetSkillByIdAsync(id);

        // Verificar se o skill pertence ao usuário
        if (skill.UserId != GetUserId())
            return Forbid();

        return Ok(skill);
    }

    [HttpPost]
    public async Task<ActionResult<SkillDto>> CreateSkill(CreateSkillDto dto)
    {
        var validationResult = await _createSkillValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var userId = GetUserId();
        var skill = await _skillService.CreateSkillAsync(userId, dto);
        return CreatedAtAction(nameof(GetSkill), new { id = skill.Id }, skill);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<SkillDto>> UpdateSkill(Guid id, UpdateSkillDto dto)
    {
        var skill = await _skillService.GetSkillByIdAsync(id);

        if (skill.UserId != GetUserId())
            return Forbid();

        var updatedSkill = await _skillService.UpdateSkillAsync(id, dto);
        return Ok(updatedSkill);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteSkill(Guid id)
    {
        var skill = await _skillService.GetSkillByIdAsync(id);

        if (skill.UserId != GetUserId())
            return Forbid();

        var deleted = await _skillService.DeleteSkillAsync(id);

        return deleted ? NoContent() : NotFound();
    }

    [HttpGet("distribution")]
    public async Task<ActionResult<IEnumerable<SkillDistributionDto>>> GetSkillDistribution()
    {
        var userId = GetUserId();
        var distribution = await _skillService.GetSkillDistributionAsync(userId);
        return Ok(distribution);
    }
}