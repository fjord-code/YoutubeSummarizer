using Microsoft.Extensions.Logging;
using YoutubeSummarizer.Application.Interfaces;
using YoutubeSummarizer.Domain.Interfaces;

namespace YoutubeSummarizer.Application.Services
{
    public class VideoSummarizationService : IVideoSummarizationService
    {
        private readonly IVideoTranscriptionService _transcriptionService;
        private readonly ISummaryService _summaryService;
        private readonly ILogger<VideoSummarizationService> _logger;

        public VideoSummarizationService(
            IVideoTranscriptionService transcriptionService,
            ISummaryService summaryService,
            ILogger<VideoSummarizationService> logger)
        {
            _transcriptionService = transcriptionService;
            _summaryService = summaryService;
            _logger = logger;
        }

        public async Task<VideoSummaryResult> SummarizeVideoAsync(string videoUrl, CancellationToken cancellationToken = default)
        {
            var requestId = Guid.NewGuid().ToString();
            
            try
            {
                _logger.LogInformation("Starting video summarization for URL: {VideoUrl}, RequestId: {RequestId}", videoUrl, requestId);

                // Step 1: Get transcription
                var transcription = await _transcriptionService.GetTranscriptionAsync(videoUrl, cancellationToken);
                
                if (string.IsNullOrWhiteSpace(transcription))
                {
                    _logger.LogWarning("No transcription available for URL: {VideoUrl}, RequestId: {RequestId}", videoUrl, requestId);
                    return new VideoSummaryResult(
                        "No transcription available for this video.",
                        "NoTranscription",
                        requestId);
                }

                _logger.LogInformation("Retrieved transcription of {Length} characters for URL: {VideoUrl}, RequestId: {RequestId}", 
                    transcription.Length, videoUrl, requestId);

                // Step 2: Generate summary
                var summary = await _summaryService.GenerateSummaryAsync(transcription, cancellationToken);
                
                _logger.LogInformation("Successfully generated summary for URL: {VideoUrl}, RequestId: {RequestId}", videoUrl, requestId);

                return new VideoSummaryResult(summary, "Success", requestId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to summarize video for URL: {VideoUrl}, RequestId: {RequestId}", videoUrl, requestId);
                
                return new VideoSummaryResult(
                    $"Failed to summarize video: {ex.Message}",
                    "Error",
                    requestId);
            }
        }
    }
} 