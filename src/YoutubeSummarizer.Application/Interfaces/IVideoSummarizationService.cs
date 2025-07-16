namespace YoutubeSummarizer.Application.Interfaces
{
    public interface IVideoSummarizationService
    {
        Task<VideoSummaryResult> SummarizeVideoAsync(string videoUrl, CancellationToken cancellationToken = default);
    }

    public record VideoSummaryResult(string Summary, string Status, string RequestId);
} 