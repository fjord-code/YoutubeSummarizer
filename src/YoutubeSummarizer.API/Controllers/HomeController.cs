using Microsoft.AspNetCore.Mvc;

namespace YoutubeSummarizer.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HomeController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new { message = "YoutubeSummarizer API is running!", timestamp = DateTime.UtcNow });
    }

    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new { status = "Healthy", timestamp = DateTime.UtcNow });
    }
} 