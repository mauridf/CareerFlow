using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareerFlow.Api.Controllers.Admin;

[ApiController]
[Route("api/v1/admin")]
[Authorize(Policy = "Admin")]
public abstract class AdminBaseController : ControllerBase
{
}
