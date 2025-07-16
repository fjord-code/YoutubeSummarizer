using YoutubeExplode;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.ClosedCaptions;
using YoutubeSummarizer.Domain.Interfaces;

namespace YoutubeSummarizer.Infrastructure.Services
{
    public class YouTubeTranscriptionService : IVideoTranscriptionService
    {
        public async Task<string> GetTranscriptionAsync(string videoUrl, CancellationToken cancellationToken = default)
        {
            var youtube = new YoutubeClient();
            var videoId = VideoId.Parse(videoUrl);
            var tracks = await youtube.Videos.ClosedCaptions.GetManifestAsync(videoId, cancellationToken);
            
            // Try to get English captions first, then fall back to any available
            ClosedCaptionTrackInfo? trackInfo = null;
            
            try
            {
                trackInfo = tracks.GetByLanguage("en");
            }
            catch (InvalidOperationException)
            {
                // English captions not available, try any available captions
                trackInfo = tracks.Tracks.FirstOrDefault();
            }
            
            if (trackInfo == null)
            {
                return string.Empty; // No captions available at all
            }

            var captions = await youtube.Videos.ClosedCaptions.GetAsync(trackInfo, cancellationToken);
            return string.Join(" ", captions.Captions.Select(c => c.Text));
        }
    }
} 