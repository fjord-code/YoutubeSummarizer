using Microsoft.Extensions.Logging;
using YoutubeSummarizer.Domain.Interfaces;
using LLama;
using LLama.Common;
using LLama.Sampling;

namespace YoutubeSummarizer.Infrastructure.Services
{
    public class LlamaSharpSummaryService : ISummaryService, IDisposable
    {
        private readonly ILogger<LlamaSharpSummaryService> _logger;
        private LLamaWeights? _model;
        private bool _disposed = false;
        private readonly bool _aiModelAvailable;
        private readonly string? _modelPath;

        public LlamaSharpSummaryService(ILogger<LlamaSharpSummaryService> logger)
        {
            _logger = logger;
            
            // Try to find and load the first available .gguf model
            _modelPath = FindFirstGgufModel();
            
            if (string.IsNullOrEmpty(_modelPath))
            {
                _logger.LogWarning("No .gguf model found in models directory. Using intelligent fallback summarization.");
                _model = null;
                _aiModelAvailable = false;
                return;
            }

            try
            {
                _logger.LogInformation("Loading LLamaSharp model from: {ModelPath}", _modelPath);
                
                var parameters = new ModelParams(_modelPath)
                {
                    ContextSize = 2048,
                    GpuLayerCount = 0 // Set to higher number if you have GPU
                };

                _model = LLamaWeights.LoadFromFile(parameters);
                _aiModelAvailable = true;
                
                _logger.LogInformation("LLamaSharp model loaded successfully from {ModelPath}", _modelPath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load LLamaSharp model from {ModelPath}", _modelPath);
                _model = null;
                _aiModelAvailable = false;
            }
        }

        private string? FindFirstGgufModel()
        {
            try
            {
                var modelsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "models");
                
                if (!Directory.Exists(modelsDirectory))
                {
                    _logger.LogWarning("Models directory not found at: {ModelsDirectory}", modelsDirectory);
                    return null;
                }

                var ggufFiles = Directory.GetFiles(modelsDirectory, "*.gguf");
                
                if (ggufFiles.Length == 0)
                {
                    _logger.LogWarning("No .gguf files found in models directory: {ModelsDirectory}", modelsDirectory);
                    return null;
                }

                var firstModel = ggufFiles.First();
                _logger.LogInformation("Found model file: {ModelFile}", Path.GetFileName(firstModel));
                
                return firstModel;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while searching for .gguf model files");
                return null;
            }
        }

        public async Task<string> GenerateSummaryAsync(string transcription, CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(transcription))
                {
                    return "No transcription available to summarize.";
                }

                _logger.LogInformation("Generating summary for transcription of {Length} characters", transcription.Length);

                if (_aiModelAvailable && _model != null)
                {
                    // Use actual LLamaSharp AI
                    var summary = await GenerateAISummaryAsync(transcription, cancellationToken);
                    _logger.LogInformation("Successfully generated AI summary using LLamaSharp");
                    return summary;
                }
                else
                {
                    // Use intelligent fallback
                    var summary = await GenerateIntelligentSummaryAsync(transcription, cancellationToken);
                    _logger.LogInformation("Generated summary using intelligent fallback");
                    return summary;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate summary for transcription");
                
                // Fallback to simple summarization if everything fails
                return await GenerateFallbackSummaryAsync(transcription, cancellationToken);
            }
        }

        private async Task<string> GenerateAISummaryAsync(string transcription, CancellationToken cancellationToken)
        {
            if (_model == null) throw new InvalidOperationException("LLamaSharp model not available");

            try
            {
                // Create a fresh context and executor for each request to avoid state issues
                var contextParams = new ModelParams(_modelPath!)
                {
                    ContextSize = 2048,
                    GpuLayerCount = 0
                };
                using var context = _model.CreateContext(contextParams);
                var executor = new InteractiveExecutor(context);

                // Create chat history with only the system prompt
                var chatHistory = new ChatHistory();
                chatHistory.AddMessage(AuthorRole.System, "You are a helpful AI assistant. Provide concise summaries of YouTube video transcriptions. Focus on main points and key takeaways. Be clear and informative. Do not include any prefixes like 'System:', 'Video:', 'Assistant:', or 'AI:' in your response. Start directly with the summary content.");

                // Create chat session
                var session = new ChatSession(executor, chatHistory);

                var inferenceParams = new InferenceParams()
                {
                    MaxTokens = 150,
                    AntiPrompts = new List<string> { "User:", "Transcription:", "[INST]", "System:", "Assistant:", "Video:" },
                    SamplingPipeline = new DefaultSamplingPipeline()
                };

                var userMessage = CreateSummarizationPrompt(transcription);
                var responseBuilder = new System.Text.StringBuilder();
                
                // Generate response streamingly
                await foreach (var text in session.ChatAsync(
                    new ChatHistory.Message(AuthorRole.User, userMessage),
                    inferenceParams))
                {
                    responseBuilder.Append(text);
                }

                var summary = responseBuilder.ToString().Trim();

                _logger.LogInformation("Generated AI summary {summary}.", summary);
                return summary;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during AI summary generation");
                throw;
            }
        }

        private string CreateSummarizationPrompt(string transcription)
        {
            // Truncate transcription if it's too long for the context
            var maxLength = 1500; // Leave room for prompt and response
            var truncatedTranscription = transcription.Length > maxLength 
                ? transcription.Substring(0, maxLength) + "..."
                : transcription;

            return $"Here is the transcription of a YouTube video:\n\n{truncatedTranscription}. Provide a concise 2-3 sentence summary of the video. Example output: 'This video covers the origins and development of the internet.'";
        }

        private async Task<string> GenerateIntelligentSummaryAsync(string transcription, CancellationToken cancellationToken)
        {
            await Task.Delay(200, cancellationToken); // Simulate AI processing time
            
            // Intelligent summarization logic
            var sentences = transcription.Split(new[] { '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries)
                                       .Where(s => !string.IsNullOrWhiteSpace(s.Trim()))
                                       .ToArray();
            
            if (sentences.Length == 0)
            {
                return "Unable to extract meaningful content for summarization.";
            }

            // Take the most important sentences (first, middle, and last)
            var importantSentences = new List<string>();
            
            if (sentences.Length >= 1)
                importantSentences.Add(sentences[0].Trim());
            
            if (sentences.Length >= 3)
                importantSentences.Add(sentences[sentences.Length / 2].Trim());
            
            if (sentences.Length >= 2)
                importantSentences.Add(sentences[sentences.Length - 1].Trim());
            
            // Remove duplicates and limit to 3 sentences
            var uniqueSentences = importantSentences.Distinct().Take(3).ToArray();
            
            var summary = string.Join(". ", uniqueSentences);
            
            if (summary.Length > 0 && !summary.EndsWith("."))
            {
                summary += ".";
            }
            
            _logger.LogInformation("Generated intelligent summary for transcription of {WordCount} words", 
                transcription.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length);
            
            return $"Intelligent Summary: {summary}";
        }

        private async Task<string> GenerateFallbackSummaryAsync(string transcription, CancellationToken cancellationToken)
        {
            await Task.Delay(100, cancellationToken); // Simulate processing time
            
            // Simple fallback summarization
            var sentences = transcription.Split(new[] { '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries)
                                       .Where(s => !string.IsNullOrWhiteSpace(s.Trim()))
                                       .Take(3)
                                       .ToArray();
            
            var summary = string.Join(". ", sentences.Select(s => s.Trim()));
            
            if (summary.Length > 0 && !summary.EndsWith("."))
            {
                summary += ".";
            }
            
            _logger.LogInformation("Generated fallback summary for transcription of {WordCount} words", 
                transcription.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length);
            
            return $"Summary: {summary}";
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                _model?.Dispose();
            }
        }
    }
} 