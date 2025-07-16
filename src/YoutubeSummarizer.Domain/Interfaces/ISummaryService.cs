namespace YoutubeSummarizer.Domain.Interfaces
{
    public interface ISummaryService
    {
        Task<string> GenerateSummaryAsync(string transcription, CancellationToken cancellationToken = default);
    }
} 