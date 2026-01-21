using Microsoft.AspNetCore.Mvc;

namespace CareerFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class ApiControllerBase : ControllerBase
{
    protected ActionResult<T> OkOrNotFound<T>(T? result)
    {
        return result == null ? NotFound() : Ok(result);
    }

    protected Guid GetUserId()
    {
        var userIdClaim = User.FindFirst("userId");
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            throw new UnauthorizedAccessException("Usuário não autenticado.");
        }
        return userId;
    }
}