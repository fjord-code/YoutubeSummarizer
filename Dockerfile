# Multi-stage build for production
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Create non-root user for security
RUN adduser --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY ["src/YoutubeSummarizer.API/YoutubeSummarizer.API.csproj", "src/YoutubeSummarizer.API/"]
COPY ["src/YoutubeSummarizer.Application/YoutubeSummarizer.Application.csproj", "src/YoutubeSummarizer.Application/"]
COPY ["src/YoutubeSummarizer.Domain/YoutubeSummarizer.Domain.csproj", "src/YoutubeSummarizer.Domain/"]
COPY ["src/YoutubeSummarizer.Infrastructure/YoutubeSummarizer.Infrastructure.csproj", "src/YoutubeSummarizer.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "src/YoutubeSummarizer.API/YoutubeSummarizer.API.csproj"

# Copy source code
COPY . .

# Build the application
WORKDIR "/src/src/YoutubeSummarizer.API"
RUN dotnet build "YoutubeSummarizer.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "YoutubeSummarizer.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app

# Copy published application
COPY --from=publish /app/publish .

# Create models directory
RUN mkdir -p models && chown -R appuser:appuser models

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost/health || exit 1

ENTRYPOINT ["dotnet", "YoutubeSummarizer.API.dll"] 