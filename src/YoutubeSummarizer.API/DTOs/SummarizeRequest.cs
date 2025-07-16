using System.ComponentModel.DataAnnotations;

namespace YoutubeSummarizer.API.DTOs;

public class SummarizeRequest
{
    [Required]
    [Url]
    public string YouTubeUrl { get; set; } = string.Empty;
} 