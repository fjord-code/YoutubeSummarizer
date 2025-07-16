using Microsoft.AspNetCore.Mvc;
using YoutubeSummarizer.Application.Interfaces;
using YoutubeSummarizer.API.DTOs;
using System.ComponentModel.DataAnnotations;

namespace YoutubeSummarizer.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SummarizeController : ControllerBase
{
    private readonly IVideoSummarizationService _summarizationService;
    private readonly ILogger<SummarizeController> _logger;
    private readonly IConfiguration _configuration;
    
    public SummarizeController(
        IVideoSummarizationService summarizationService, 
        ILogger<SummarizeController> logger,
        IConfiguration configuration)
    {
        _summarizationService = summarizationService;
        _logger = logger;
        _configuration = configuration;
    }

    [HttpPost]
    [ProducesResponseType(typeof(SummarizeResponse), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public async Task<IActionResult> SummarizeVideo(
        [FromBody] SummarizeRequest request, 
        CancellationToken cancellationToken)
    {
        var requestId = Guid.NewGuid().ToString();
        var startTime = DateTime.UtcNow;
        var timeoutSeconds = _configuration.GetValue<int>("Performance:RequestTimeoutSeconds", 300);
        CancellationTokenSource? timeoutCts = null;
        try
        {
            _logger.LogInformation("Starting video summarization request {RequestId} for URL: {YouTubeUrl}", 
                requestId, request.YouTubeUrl);

            // Validate request
            if (string.IsNullOrWhiteSpace(request.YouTubeUrl))
            {
                _logger.LogWarning("Invalid request {RequestId}: YouTube URL is empty", requestId);
                return BadRequest(new ErrorResponse
                {
                    Error = "YouTube URL is required",
                    RequestId = requestId,
                    Timestamp = DateTime.UtcNow
                });
            }

            if (!IsValidYouTubeUrl(request.YouTubeUrl))
            {
                _logger.LogWarning("Invalid request {RequestId}: Invalid YouTube URL format", requestId);
                return BadRequest(new ErrorResponse
                {
                    Error = "Invalid YouTube URL format",
                    RequestId = requestId,
                    Timestamp = DateTime.UtcNow
                });
            }

            // Create a timeout cancellation token
            timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutSeconds));
            using var combinedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

            var result = await _summarizationService.SummarizeVideoAsync(request.YouTubeUrl, combinedCts.Token);

            var response = new SummarizeResponse
            {
                Message = result.Summary,
                Status = result.Status,
                RequestId = requestId,
                ProcessingTimeMs = (DateTime.UtcNow - startTime).TotalMilliseconds
            };

            _logger.LogInformation("Successfully completed summarization request {RequestId} in {ProcessingTime}ms", 
                requestId, response.ProcessingTimeMs);

            return Ok(response);
        }
        catch (OperationCanceledException) when (timeoutCts != null && timeoutCts.Token.IsCancellationRequested)
        {
            _logger.LogWarning("Request {RequestId} timed out after {TimeoutSeconds} seconds", 
                requestId, timeoutSeconds);
            return StatusCode(408, new ErrorResponse
            {
                Error = "Request timed out",
                RequestId = requestId,
                Timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to summarize video for request {RequestId} with URL: {YouTubeUrl}", 
                requestId, request.YouTubeUrl);
            
            var errorResponse = new ErrorResponse
            {
                Error = "An unexpected error occurred while processing your request",
                RequestId = requestId,
                Timestamp = DateTime.UtcNow
            };

            // In development, include more details
            if (HttpContext.RequestServices.GetService<IWebHostEnvironment>()?.IsDevelopment() == true)
            {
                errorResponse.Error = ex.Message;
            }

            return StatusCode(500, errorResponse);
        }
        finally
        {
            timeoutCts?.Dispose();
        }
    }

    private static bool IsValidYouTubeUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return false;

        try
        {
            var uri = new Uri(url);
            return uri.Host.Contains("youtube.com") || uri.Host.Contains("youtu.be");
        }
        catch
        {
            return false;
        }
    }
} 