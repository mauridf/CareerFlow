using CareerFlow.Application.DTOs;
using CareerFlow.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareerFlow.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AcademicsController : ApiControllerBase
{
    private readonly IAcademicService _academicService;

    public AcademicsController(IAcademicService academicService)
    {
        _academicService = academicService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AcademicBackgroundDto>>> GetAcademics()
    {
        var userId = GetUserId();
        var academics = await _academicService.GetUserAcademicsAsync(userId);
        return Ok(academics);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AcademicBackgroundDto>> GetAcademic(Guid id)
    {
        var academic = await _academicService.GetAcademicByIdAsync(id);

        if (academic.UserId != GetUserId())
            return Forbid();

        return Ok(academic);
    }

    [HttpPost]
    public async Task<ActionResult<AcademicBackgroundDto>> CreateAcademic(CreateAcademicBackgroundDto dto)
    {
        var userId = GetUserId();
        var academic = await _academicService.CreateAcademicAsync(userId, dto);
        return CreatedAtAction(nameof(GetAcademic), new { id = academic.Id }, academic);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<AcademicBackgroundDto>> UpdateAcademic(Guid id, UpdateAcademicBackgroundDto dto)
    {
        var academic = await _academicService.GetAcademicByIdAsync(id);

        if (academic.UserId != GetUserId())
            return Forbid();

        var updatedAcademic = await _academicService.UpdateAcademicAsync(id, dto);
        return Ok(updatedAcademic);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAcademic(Guid id)
    {
        var academic = await _academicService.GetAcademicByIdAsync(id);

        if (academic.UserId != GetUserId())
            return Forbid();

        var deleted = await _academicService.DeleteAcademicAsync(id);

        return deleted ? NoContent() : NotFound();
    }
}