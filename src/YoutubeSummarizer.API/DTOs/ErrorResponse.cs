namespace YoutubeSummarizer.API.DTOs;

public class ErrorResponse
{
    public string Error { get; set; } = string.Empty;
    public string RequestId { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
} 