using CareerFlow.Application.DTOs;
using CareerFlow.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareerFlow.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CertificatesController : ApiControllerBase
{
    private readonly ICertificateService _certificateService;

    public CertificatesController(ICertificateService certificateService)
    {
        _certificateService = certificateService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CertificateDto>>> GetCertificates()
    {
        var userId = GetUserId();
        var certificates = await _certificateService.GetUserCertificatesAsync(userId);
        return Ok(certificates);
    }

    [HttpGet("expiring")]
    public async Task<ActionResult<IEnumerable<CertificateDto>>> GetExpiringCertificates([FromQuery] int daysThreshold = 30)
    {
        var userId = GetUserId();
        var certificates = await _certificateService.GetExpiringCertificatesAsync(userId, daysThreshold);
        return Ok(certificates);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CertificateDto>> GetCertificate(Guid id)
    {
        var certificate = await _certificateService.GetCertificateByIdAsync(id);

        if (certificate.UserId != GetUserId())
            return Forbid();

        return Ok(certificate);
    }

    [HttpPost]
    public async Task<ActionResult<CertificateDto>> CreateCertificate(CreateCertificateDto dto)
    {
        var userId = GetUserId();
        var certificate = await _certificateService.CreateCertificateAsync(userId, dto);
        return CreatedAtAction(nameof(GetCertificate), new { id = certificate.Id }, certificate);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<CertificateDto>> UpdateCertificate(Guid id, UpdateCertificateDto dto)
    {
        var certificate = await _certificateService.GetCertificateByIdAsync(id);

        if (certificate.UserId != GetUserId())
            return Forbid();

        var updatedCertificate = await _certificateService.UpdateCertificateAsync(id, dto);
        return Ok(updatedCertificate);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteCertificate(Guid id)
    {
        var certificate = await _certificateService.GetCertificateByIdAsync(id);

        if (certificate.UserId != GetUserId())
            return Forbid();

        var deleted = await _certificateService.DeleteCertificateAsync(id);

        return deleted ? NoContent() : NotFound();
    }
}