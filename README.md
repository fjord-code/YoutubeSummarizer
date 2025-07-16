# YoutubeSummarizer

A cursor vibe coded test project.

A .NET web application following Clean Architecture principles with a Web API backend and Blazor Server frontend for summarizing YouTube videos using AI.

## Features

- **YouTube Video Transcription**: Extract captions/transcripts from YouTube videos
- **AI-Powered Summarization**: Generate intelligent 2-3 sentence summaries using LLamaSharp
- **Clean Architecture**: Well-structured, maintainable codebase
- **Modern UI**: Beautiful Blazor Server frontend with glass morphism design
- **RESTful API**: Web API backend for video processing
- **Intelligent Fallback**: Smart summarization when AI models aren't available

## Project Structure

This solution follows Clean Architecture principles with the following layers:

```
src/
├── YoutubeSummarizer.Domain/           # Core business entities and interfaces
├── YoutubeSummarizer.Application/      # Business logic and use cases
├── YoutubeSummarizer.Infrastructure/   # External concerns (database, APIs, etc.)
├── YoutubeSummarizer.API/              # Web API presentation layer
└── YoutubeSummarizer.Blazor/           # Blazor Server presentation layer
```

## Architecture Principles

### Dependency Direction
- **Domain** ← **Application** ← **Infrastructure**
- **Domain** ← **Application** ← **API/Blazor**
- **Infrastructure** ← **API/Blazor**

### Layer Responsibilities

#### Domain Layer
- Contains business entities
- Defines repository interfaces
- Contains domain services
- No dependencies on other layers

#### Application Layer
- Contains use cases and business logic
- Implements application services
- Orchestrates domain objects
- Depends on Domain layer

#### Infrastructure Layer
- Implements repository interfaces
- Handles external concerns (database, APIs, file system)
- Contains concrete implementations
- Depends on Domain layer

#### Presentation Layer (API & Blazor)
- Handles HTTP requests and responses
- Contains controllers and pages
- Configures dependency injection
- Depends on Application and Infrastructure layers

## Getting Started

### Prerequisites
- .NET 8.0 SDK
- Visual Studio 2022 or VS Code

### Running the Application

1. **Restore dependencies:**
   ```bash
   dotnet restore
   ```

2. **Build the solution:**
   ```bash
   dotnet build
   ```

3. **Run the API:**
   ```bash
   cd src/YoutubeSummarizer.API
   dotnet run
   ```

4. **Run the Blazor application:**
   ```bash
   cd src/YoutubeSummarizer.Blazor
   dotnet run
   ```

## YouTube Transcription & AI Summarization

This application uses **YoutubeExplode** for transcription and **LLamaSharp** for AI-powered summarization.

### How it works:
1. User enters a YouTube video URL in the Blazor frontend
2. The request is sent to the API endpoint `/api/summarize`
3. The API uses YoutubeExplode to extract available captions
4. English captions are preferred, but any available language is used
5. The transcription is sent to LLamaSharp for AI summarization
6. A concise 2-3 sentence summary is returned to the frontend

### AI Summarization Features:
- **LLamaSharp Integration**: Uses local AI models for privacy and performance
- **Intelligent Fallback**: Smart summarization when AI models aren't available
- **Context-Aware**: Extracts key points from first, middle, and last sentences
- **Configurable**: Easy to adjust summary length and style

### LLamaSharp Setup (Optional):
1. Download a GGUF model file (e.g., llama-2-7b-chat.gguf)
2. Place it in `src/YoutubeSummarizer.API/models/`
3. Restart the application
4. The service will automatically use the AI model

### Supported Features:
- Automatic caption detection
- English language preference
- Fallback to any available language
- Error handling for videos without captions
- Intelligent summarization algorithms
- Comprehensive logging and monitoring

## Development Guidelines

### Code Style
- Use PascalCase for classes and methods
- Use camelCase for private fields (with underscore prefix)
- Interface names start with 'I'
- Use async/await for I/O operations
- Add 'Async' suffix to async methods

### Clean Architecture Rules
- Never reference Infrastructure from Domain
- Never use DbContext directly in Controllers
- Use dependency injection for all services
- Keep business logic in Application layer
- Keep external concerns in Infrastructure layer

## Technology Stack

- **.NET 8.0** - Framework
- **ASP.NET Core Web API** - Backend API
- **Blazor Server** - Frontend
- **YoutubeExplode** - YouTube video processing
- **LLamaSharp** - AI-powered summarization
- **Dependency Injection** - IoC container
- **Swagger/OpenAPI** - API documentation

## API Endpoints

### POST /api/summarize
Extracts transcription and generates AI summary from a YouTube video.

**Request Body:**
```json
{
  "youtubeUrl": "https://www.youtube.com/watch?v=VIDEO_ID"
}
```

**Response:**
```json
{
  "message": "AI-generated summary of the video content...",
  "status": "Success",
  "requestId": "guid"
}
```

## Contributing

1. Follow Clean Architecture principles
2. Maintain separation of concerns
3. Use dependency injection
4. Write clean, readable code
5. Test your changes thoroughly
6. Add comprehensive logging 