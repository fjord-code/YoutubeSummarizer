#!/usr/bin/env pwsh

param(
    [string]$Environment = "Production",
    [switch]$SkipBuild,
    [switch]$SkipTests,
    [switch]$Help
)

if ($Help) {
    Write-Host @"
YouTube Summarizer Deployment Script

Usage: .\deploy.ps1 [Options]

Options:
    -Environment <string>    Deployment environment (Production, Staging, Development)
    -SkipBuild              Skip building the application
    -SkipTests              Skip running tests
    -Help                   Show this help message

Examples:
    .\deploy.ps1                           # Deploy to production
    .\deploy.ps1 -Environment Staging      # Deploy to staging
    .\deploy.ps1 -SkipTests                # Deploy without running tests
"@
    exit 0
}

Write-Host "🚀 Starting YouTube Summarizer deployment..." -ForegroundColor Green
Write-Host "Environment: $Environment" -ForegroundColor Yellow

# Check prerequisites
Write-Host "📋 Checking prerequisites..." -ForegroundColor Blue

if (-not (Get-Command "dotnet" -ErrorAction SilentlyContinue)) {
    Write-Error "❌ .NET SDK is not installed or not in PATH"
    exit 1
}

if (-not (Get-Command "docker" -ErrorAction SilentlyContinue)) {
    Write-Error "❌ Docker is not installed or not in PATH"
    exit 1
}

Write-Host "✅ Prerequisites check passed" -ForegroundColor Green

# Clean previous builds
Write-Host "🧹 Cleaning previous builds..." -ForegroundColor Blue
dotnet clean
if ($LASTEXITCODE -ne 0) {
    Write-Error "❌ Failed to clean solution"
    exit 1
}

# Restore packages
Write-Host "📦 Restoring packages..." -ForegroundColor Blue
dotnet restore
if ($LASTEXITCODE -ne 0) {
    Write-Error "❌ Failed to restore packages"
    exit 1
}

# Build solution
if (-not $SkipBuild) {
    Write-Host "🔨 Building solution..." -ForegroundColor Blue
    dotnet build -c Release --no-restore
    if ($LASTEXITCODE -ne 0) {
        Write-Error "❌ Build failed"
        exit 1
    }
    Write-Host "✅ Build completed successfully" -ForegroundColor Green
}

# Run tests (if not skipped)
if (-not $SkipTests) {
    Write-Host "🧪 Running tests..." -ForegroundColor Blue
    # Add test execution here when tests are implemented
    Write-Host "⚠️  No tests configured yet" -ForegroundColor Yellow
}

# Publish applications
Write-Host "📤 Publishing applications..." -ForegroundColor Blue

# Publish API
Write-Host "  📡 Publishing API..." -ForegroundColor Cyan
dotnet publish "src/YoutubeSummarizer.API/YoutubeSummarizer.API.csproj" -c Release -o "publish/api" --no-build
if ($LASTEXITCODE -ne 0) {
    Write-Error "❌ Failed to publish API"
    exit 1
}

# Publish Blazor
Write-Host "  🌐 Publishing Blazor..." -ForegroundColor Cyan
dotnet publish "src/YoutubeSummarizer.Blazor/YoutubeSummarizer.Blazor.csproj" -c Release -o "publish/blazor" --no-build
if ($LASTEXITCODE -ne 0) {
    Write-Error "❌ Failed to publish Blazor"
    exit 1
}

Write-Host "✅ Applications published successfully" -ForegroundColor Green

# Build Docker images
Write-Host "🐳 Building Docker images..." -ForegroundColor Blue

# Build API image
Write-Host "  📡 Building API image..." -ForegroundColor Cyan
docker build -f Dockerfile -t youtube-summarizer-api:latest .
if ($LASTEXITCODE -ne 0) {
    Write-Error "❌ Failed to build API Docker image"
    exit 1
}

# Build Blazor image
Write-Host "  🌐 Building Blazor image..." -ForegroundColor Cyan
docker build -f Dockerfile.blazor -t youtube-summarizer-blazor:latest .
if ($LASTEXITCODE -ne 0) {
    Write-Error "❌ Failed to build Blazor Docker image"
    exit 1
}

Write-Host "✅ Docker images built successfully" -ForegroundColor Green

# Create deployment directory
$deployDir = "deploy/$Environment"
if (Test-Path $deployDir) {
    Remove-Item $deployDir -Recurse -Force
}
New-Item -ItemType Directory -Path $deployDir -Force | Out-Null

# Copy published files
Write-Host "📁 Preparing deployment package..." -ForegroundColor Blue
Copy-Item "publish/api/*" -Destination "$deployDir/api" -Recurse -Force
Copy-Item "publish/blazor/*" -Destination "$deployDir/blazor" -Recurse -Force

# Copy configuration files
Copy-Item "docker-compose.yml" -Destination "$deployDir/" -Force
Copy-Item "Dockerfile" -Destination "$deployDir/" -Force
Copy-Item "Dockerfile.blazor" -Destination "$deployDir/" -Force

# Create deployment info
$deployInfo = @{
    Environment = $Environment
    DeployedAt = Get-Date -Format "yyyy-MM-dd HH:mm:ss UTC"
    Version = "1.0.0"
    Commit = git rev-parse HEAD 2>$null
} | ConvertTo-Json

$deployInfo | Out-File "$deployDir/deployment-info.json" -Encoding UTF8

Write-Host "✅ Deployment package created at: $deployDir" -ForegroundColor Green

# Display deployment summary
Write-Host "`n📊 Deployment Summary:" -ForegroundColor Green
Write-Host "  Environment: $Environment" -ForegroundColor White
Write-Host "  API: Ready for deployment" -ForegroundColor White
Write-Host "  Blazor: Ready for deployment" -ForegroundColor White
Write-Host "  Docker Images: Built successfully" -ForegroundColor White
Write-Host "  Package Location: $deployDir" -ForegroundColor White

Write-Host "`n🚀 Deployment completed successfully!" -ForegroundColor Green
Write-Host "`nNext steps:" -ForegroundColor Yellow
Write-Host "  1. Copy the deployment package to your production server" -ForegroundColor White
Write-Host "  2. Run: docker-compose up -d" -ForegroundColor White
Write-Host "  3. Monitor the application at: http://localhost:5000" -ForegroundColor White
Write-Host "  4. Check health status at: http://localhost:5000/health" -ForegroundColor White 