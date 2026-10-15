using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace VivesRental.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TimeController : ControllerBase
{
    [HttpGet("utcnow")]
    public ActionResult<DateTime> GetUtcNow()
    {
        return Ok(DateTime.UtcNow);
    }
}
