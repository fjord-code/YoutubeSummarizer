#!/usr/bin/env pwsh

Write-Host "🚀 Starting YouTube Summarizer Applications..." -ForegroundColor Green

# Function to check if a port is in use
function Test-Port {
    param([int]$Port)
    try {
        $connection = New-Object System.Net.Sockets.TcpClient
        $connection.Connect("localhost", $Port)
        $connection.Close()
        return $true
    }
    catch {
        return $false
    }
}

# Check if ports are available
Write-Host "📋 Checking port availability..." -ForegroundColor Blue

if (Test-Port 5001) {
    Write-Warning "⚠️  Port 5001 is already in use. The API might not start properly."
}

if (Test-Port 7123) {
    Write-Warning "⚠️  Port 7123 is already in use. The Blazor app might not start properly."
}

Write-Host "✅ Port check completed" -ForegroundColor Green

# Start API in background
Write-Host "📡 Starting API on https://localhost:5001..." -ForegroundColor Cyan
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$PWD'; dotnet run --project src/YoutubeSummarizer.API --urls 'https://localhost:5001;http://localhost:5000'"

# Wait a moment for API to start
Start-Sleep -Seconds 3

# Start Blazor in background
Write-Host "🌐 Starting Blazor on https://localhost:7123..." -ForegroundColor Cyan
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$PWD'; dotnet run --project src/YoutubeSummarizer.Blazor --urls 'https://localhost:7123;http://localhost:5196'"

Write-Host "`n🎉 Applications started successfully!" -ForegroundColor Green
Write-Host "`n📊 Application URLs:" -ForegroundColor Yellow
Write-Host "  API: https://localhost:5001" -ForegroundColor White
Write-Host "  API Health: https://localhost:5001/health" -ForegroundColor White
Write-Host "  API Swagger: https://localhost:5001/swagger" -ForegroundColor White
Write-Host "  Blazor: https://localhost:7123" -ForegroundColor White

Write-Host "`n💡 Tips:" -ForegroundColor Yellow
Write-Host "  - Wait a few seconds for both applications to fully start" -ForegroundColor White
Write-Host "  - Check the health endpoint to verify API is running" -ForegroundColor White
Write-Host "  - Close the PowerShell windows to stop the applications" -ForegroundColor White

Write-Host "`n⏳ Waiting for applications to be ready..." -ForegroundColor Blue
Start-Sleep -Seconds 5

# Try to open the applications in browser
try {
    Start-Process "https://localhost:7123"
    Write-Host "✅ Opened Blazor application in browser" -ForegroundColor Green
}
catch {
    Write-Host "⚠️  Could not open browser automatically. Please navigate to https://localhost:7123" -ForegroundColor Yellow
} 