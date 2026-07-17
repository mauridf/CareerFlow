using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CareerFlow.Application.Features.Certificates.Commands;
using CareerFlow.Application.Features.Certificates.DTOs;
using CareerFlow.Application.Features.Certificates.Queries;

namespace CareerFlow.Api.Controllers;

[ApiController]
[Route("api/v1/profile/certificates")]
[Authorize]
public class CertificatesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CertificatesController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetCertificates()
    {
        var result = await _mediator.Send(new GetCertificatesQuery());
        return Ok(new { success = true, data = result, meta = new { timestamp = DateTime.UtcNow } });
    }

    [HttpPost]
    public async Task<IActionResult> CreateCertificate([FromBody] CreateCertificateRequest request)
    {
        var cmd = new CreateCertificateCommand(request.Title, request.Issuer, request.IssueDate,
            request.ExpirationDate, request.CertificateId, request.CredentialId, request.CredentialUrl);
        var result = await _mediator.Send(cmd);
        return Created($"/api/v1/profile/certificates/{result.Id}", new { success = true, data = result, meta = new { timestamp = DateTime.UtcNow } });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateCertificate(Guid id, [FromBody] UpdateCertificateRequest request)
    {
        var cmd = new UpdateCertificateCommand(id, request.Title, request.Issuer, request.IssueDate,
            request.ExpirationDate, request.CertificateId, request.CredentialUrl);
        var result = await _mediator.Send(cmd);
        return Ok(new { success = true, data = result, meta = new { timestamp = DateTime.UtcNow } });
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteCertificate(Guid id)
    {
        await _mediator.Send(new DeleteCertificateCommand(id));
        return NoContent();
    }
}
