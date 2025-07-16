# YoutubeSummarizer

A cursor vibe coded test project.

A .NET web application following Clean Architecture principles with a Web API backend and Blazor Server frontend.

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
- Depends only on Domain layer

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
- **Entity Framework Core** - Data access (when added)
- **Dependency Injection** - IoC container 