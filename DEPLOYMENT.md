# YouTube Summarizer - Production Deployment Guide

This guide provides step-by-step instructions for deploying the YouTube Summarizer application to production environments.

## Prerequisites

- .NET 8.0 SDK
- Docker and Docker Compose
- PowerShell (for Windows deployment scripts)
- Git

## Quick Start

### 1. Automated Deployment (Recommended)

Use the provided PowerShell deployment script:

```powershell
# Deploy to production
.\deploy.ps1

# Deploy to staging
.\deploy.ps1 -Environment Staging

# Deploy without running tests
.\deploy.ps1 -SkipTests
```

### 2. Manual Deployment

#### Step 1: Build and Publish

```bash
# Clean and restore
dotnet clean
dotnet restore

# Build in Release mode
dotnet build -c Release

# Publish applications
dotnet publish src/YoutubeSummarizer.API/YoutubeSummarizer.API.csproj -c Release -o publish/api
dotnet publish src/YoutubeSummarizer.Blazor/YoutubeSummarizer.Blazor.csproj -c Release -o publish/blazor
```

#### Step 2: Docker Deployment

```bash
# Build Docker images
docker build -f Dockerfile -t youtube-summarizer-api:latest .
docker build -f Dockerfile.blazor -t youtube-summarizer-blazor:latest .

# Run with Docker Compose
docker-compose up -d
```

## Configuration

### Environment Variables

| Variable | Description | Default | Required |
|----------|-------------|---------|----------|
| `ASPNETCORE_ENVIRONMENT` | Environment name | Production | Yes |
| `ASPNETCORE_URLS` | Application URLs | http://+:80;https://+:443 | No |

### Configuration Files

#### appsettings.Production.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.Hosting.Lifetime": "Information",
      "YoutubeSummarizer": "Information"
    }
  },
  "Cors": {
    "AllowedOrigins": [
      "https://yourdomain.com",
      "https://www.yourdomain.com"
    ]
  },
  "Security": {
    "EnableHttpsRedirection": true,
    "EnableSecurityHeaders": true
  },
  "Performance": {
    "MaxConcurrentRequests": 10,
    "RequestTimeoutSeconds": 300
  }
}
```

### AI Model Setup

1. Download a GGUF model file (e.g., `llama-2-7b-chat.gguf`)
2. Place it in the `models/` directory
3. The application will automatically detect and use the model

## Monitoring and Health Checks

### Health Check Endpoints

- **Overall Health**: `GET /health`
- **Readiness**: `GET /health/ready`
- **Liveness**: `GET /health/live`

### Logging

The application uses Serilog for structured logging:

- **Console**: Structured JSON output
- **File**: Daily rolling logs in `logs/` directory
- **Retention**: 30 days of logs

### Metrics

Monitor the following key metrics:

- Request processing time
- Error rates
- Memory usage
- CPU usage
- AI model availability

## Security Considerations

### Production Security Checklist

- [ ] HTTPS is enabled and properly configured
- [ ] Security headers are enabled
- [ ] CORS is configured with specific origins
- [ ] Rate limiting is enabled
- [ ] Non-root user is used in Docker containers
- [ ] Environment variables are properly set
- [ ] Logs don't contain sensitive information
- [ ] AI models are stored securely

### Security Headers

The application automatically adds the following security headers in production:

- `X-Content-Type-Options: nosniff`
- `X-Frame-Options: DENY`
- `X-XSS-Protection: 1; mode=block`
- `Referrer-Policy: strict-origin-when-cross-origin`
- `Content-Security-Policy: default-src 'self'`

## Performance Optimization

### Configuration Tuning

1. **Rate Limiting**: Adjust `MaxConcurrentRequests` based on server capacity
2. **Request Timeout**: Set `RequestTimeoutSeconds` based on expected processing time
3. **AI Model**: Use appropriate model size for your hardware

### Resource Requirements

#### Minimum Requirements

- **CPU**: 2 cores
- **Memory**: 4GB RAM
- **Storage**: 10GB (without AI models)
- **Network**: 10 Mbps

#### Recommended Requirements

- **CPU**: 4+ cores
- **Memory**: 8GB+ RAM
- **Storage**: 50GB+ (with AI models)
- **Network**: 100 Mbps+

## Troubleshooting

### Common Issues

#### 1. Application Won't Start

```bash
# Check logs
docker logs youtube-summarizer-api

# Check health status
curl http://localhost:5000/health
```

#### 2. AI Model Not Loading

```bash
# Check if model file exists
ls -la models/

# Check application logs for model loading errors
docker logs youtube-summarizer-api | grep -i model
```

#### 3. High Memory Usage

- Reduce AI model size
- Increase server memory
- Monitor for memory leaks

#### 4. Slow Response Times

- Check network connectivity
- Monitor CPU usage
- Consider upgrading hardware
- Optimize AI model configuration

### Log Analysis

```bash
# View recent logs
tail -f logs/youtube-summarizer-*.log

# Search for errors
grep -i error logs/youtube-summarizer-*.log

# Monitor request times
grep "ProcessingTimeMs" logs/youtube-summarizer-*.log
```

## Backup and Recovery

### Backup Strategy

1. **Application Code**: Version controlled in Git
2. **Configuration**: Backed up with application
3. **AI Models**: Stored separately (large files)
4. **Logs**: Rotated daily, retained for 30 days

### Recovery Procedures

1. **Application Recovery**: Redeploy from Git
2. **Configuration Recovery**: Restore from backup
3. **Data Recovery**: Re-download AI models if needed

## Scaling

### Horizontal Scaling

1. **Load Balancer**: Use nginx or similar
2. **Multiple Instances**: Deploy multiple containers
3. **Database**: Consider adding caching layer

### Vertical Scaling

1. **Increase Resources**: More CPU, memory, storage
2. **Optimize Configuration**: Tune performance settings
3. **Better Hardware**: Use dedicated servers

## Maintenance

### Regular Tasks

- **Log Rotation**: Automatic (30-day retention)
- **Security Updates**: Monthly dependency updates
- **Performance Monitoring**: Continuous monitoring
- **Backup Verification**: Weekly backup tests

### Update Procedures

1. **Code Updates**: Use deployment script
2. **Configuration Changes**: Update appsettings files
3. **AI Model Updates**: Replace model files and restart

## Support

For issues and questions:

1. Check the troubleshooting section
2. Review application logs
3. Check health endpoints
4. Contact the development team

## Version History

- **v1.0.0**: Initial production release
  - Clean Architecture implementation
  - Docker containerization
  - Health checks and monitoring
  - Security hardening
  - Structured logging 