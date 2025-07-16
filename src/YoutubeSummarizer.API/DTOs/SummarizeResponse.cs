namespace YoutubeSummarizer.API.DTOs;

public class SummarizeResponse
{
    public string Message { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string RequestId { get; set; } = string.Empty;
    public double ProcessingTimeMs { get; set; }
} 