using CareerFlow.Application.DTOs;
using CareerFlow.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareerFlow.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ProfileController : ApiControllerBase
{
    private readonly IProfileService _profileService;

    public ProfileController(IProfileService profileService)
    {
        _profileService = profileService;
    }

    [HttpGet("summary")]
    public async Task<ActionResult<ProfessionalSummaryDto>> GetSummary()
    {
        var userId = GetUserId();
        var summary = await _profileService.GetSummaryAsync(userId);
        return OkOrNotFound(summary);
    }

    [HttpPost("summary")]
    public async Task<ActionResult<ProfessionalSummaryDto>> CreateOrUpdateSummary(CreateProfessionalSummaryDto dto)
    {
        var userId = GetUserId();
        var summary = await _profileService.CreateOrUpdateSummaryAsync(userId, dto);
        return Ok(summary);
    }

    [HttpDelete("summary")]
    public async Task<ActionResult> DeleteSummary()
    {
        var userId = GetUserId();
        var deleted = await _profileService.DeleteSummaryAsync(userId);

        return deleted ? NoContent() : NotFound();
    }

    [HttpGet("social-medias")]
    public async Task<ActionResult<IEnumerable<SocialMediaDto>>> GetSocialMedias()
    {
        var userId = GetUserId();
        var socialMedias = await _profileService.GetSocialMediasAsync(userId);
        return Ok(socialMedias);
    }

    [HttpPost("social-medias")]
    public async Task<ActionResult<SocialMediaDto>> AddSocialMedia(CreateSocialMediaDto dto)
    {
        var userId = GetUserId();
        var socialMedia = await _profileService.AddSocialMediaAsync(userId, dto);
        return CreatedAtAction(nameof(GetSocialMedias), socialMedia);
    }

    [HttpDelete("social-medias/{id}")]
    public async Task<ActionResult> RemoveSocialMedia(Guid id)
    {
        var userId = GetUserId();
        var removed = await _profileService.RemoveSocialMediaAsync(userId, id);

        return removed ? NoContent() : NotFound();
    }

    [HttpGet("dashboard/stats")]
    public async Task<ActionResult<DashboardStatsDto>> GetDashboardStats()
    {
        var userId = GetUserId();
        var stats = await _profileService.GetDashboardStatsAsync(userId);
        return Ok(stats);
    }

    [HttpGet("resume")]
    public async Task<ActionResult<ResumeDataDto>> GetResumeData()
    {
        var userId = GetUserId();
        var resumeData = await _profileService.GetResumeDataAsync(userId);
        return Ok(resumeData);
    }
}