using Microsoft.AspNetCore.Mvc;

namespace YoutubeSummarizer.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SummarizeController : ControllerBase
{
    [HttpPost]
    public IActionResult SummarizeVideo([FromBody] SummarizeRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.YouTubeUrl))
        {
            return BadRequest(new { error = "YouTube URL is required" });
        }

        var response = new SummarizeResponse
        {
            Message = $"Got an url: {request.YouTubeUrl} at date time: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss UTC}. Started processing.",
            Status = "Processing",
            RequestId = Guid.NewGuid().ToString()
        };

        return Ok(response);
    }
}

public class SummarizeRequest
{
    public string YouTubeUrl { get; set; } = string.Empty;
}

public class SummarizeResponse
{
    public string Message { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string RequestId { get; set; } = string.Empty;
} 