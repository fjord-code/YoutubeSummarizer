namespace YoutubeSummarizer.Domain.Interfaces
{
    public interface IVideoTranscriptionService
    {
        Task<string> GetTranscriptionAsync(string videoUrl, CancellationToken cancellationToken = default);
    }
} 