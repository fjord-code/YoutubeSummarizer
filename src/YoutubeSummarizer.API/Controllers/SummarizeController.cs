using Microsoft.AspNetCore.Mvc;
using YoutubeSummarizer.Domain.Interfaces;

namespace YoutubeSummarizer.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SummarizeController : ControllerBase
{
    private readonly IVideoTranscriptionService _transcriptionService;
    private readonly ILogger<SummarizeController> _logger;
    
    public SummarizeController(IVideoTranscriptionService transcriptionService, ILogger<SummarizeController> logger)
    {
        _transcriptionService = transcriptionService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> SummarizeVideo([FromBody] SummarizeRequest request, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.YouTubeUrl))
            {
                return BadRequest(new { error = "YouTube URL is required" });
            }

            var transcription = string.Empty;
            transcription = await _transcriptionService.GetTranscriptionAsync(request.YouTubeUrl, cancellationToken);

            _logger.LogInformation("Transcription: {Transcription}", transcription);

            var response = new SummarizeResponse
            {
                Message = transcription,
                Status = string.IsNullOrWhiteSpace(transcription) ? "NoTranscription" : "Success",
                RequestId = Guid.NewGuid().ToString()
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve transcription for URL: {YouTubeUrl}", request.YouTubeUrl);
            var errorResponse = new SummarizeResponse
            {
                Message = $"Failed to retrieve transcription: {ex.Message}",
                Status = "Error",
                RequestId = Guid.NewGuid().ToString()
            };
            return StatusCode(500, errorResponse);
        }
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