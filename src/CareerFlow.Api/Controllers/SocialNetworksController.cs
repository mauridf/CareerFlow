using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CareerFlow.Application.Features.SocialNetworks.Commands;
using CareerFlow.Application.Features.SocialNetworks.DTOs;
using CareerFlow.Application.Features.SocialNetworks.Queries;
using CareerFlow.Api.Helpers;

namespace CareerFlow.Api.Controllers;

[ApiController]
[Route("api/v1/profile/social-networks")]
[Authorize]
public class SocialNetworksController : ControllerBase
{
    private readonly IMediator _mediator;

    public SocialNetworksController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<IActionResult> GetSocialNetworks()
    {
        var result = await _mediator.Send(new GetSocialNetworksQuery());
        return ResponseHelper.OkResponse(result, HttpContext);
    }

    [HttpPost]
    public async Task<IActionResult> CreateSocialNetwork([FromBody] CreateSocialNetworkRequest request)
    {
        var cmd = new CreateSocialNetworkCommand(request.NetworkType, request.Url, request.DisplayOrder);
        var result = await _mediator.Send(cmd);
        return ResponseHelper.CreatedResponse($"/api/v1/profile/social-networks/{result.Id}", result, HttpContext);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateSocialNetwork(Guid id, [FromBody] UpdateSocialNetworkRequest request)
    {
        var cmd = new UpdateSocialNetworkCommand(id, request.NetworkType, request.Url, request.DisplayOrder);
        var result = await _mediator.Send(cmd);
        return ResponseHelper.OkResponse(result, HttpContext);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteSocialNetwork(Guid id)
    {
        await _mediator.Send(new DeleteSocialNetworkCommand(id));
        return NoContent();
    }
}
